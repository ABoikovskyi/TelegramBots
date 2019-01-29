using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBots.Context;
using TelegramBots.Helpers;
using User = DataLayer.Models.User;

namespace TelegramBots.Services
{
	public static class PopCornBotService
	{
		public static TelegramBotClient Client;
		public const string AppLink = "https://playzone.ua";
		public static IServiceProvider ServiceProvider;
		public static ReplyKeyboardMarkup MainKeyboard;
		public static ReplyKeyboardMarkup ConcertsChoiceKeyboard;
		public static ReplyKeyboardMarkup ConcertKeyboard;
		public static Dictionary<long, int> UserCurrentConcert = new Dictionary<long, int>();
		public static Dictionary<long, string> UserCurrentConcertsType = new Dictionary<long, string>();

		static PopCornBotService()
		{
			MainKeyboard = new ReplyKeyboardMarkup
			{
				Keyboard = new[]
				{
					new KeyboardButton[] {PhraseHelper.About, PhraseHelper.Contacts},
					new KeyboardButton[] {PhraseHelper.Tickets},
					new KeyboardButton[] {PhraseHelper.Concerts},
					new KeyboardButton[] {PhraseHelper.Sales}
				},
				ResizeKeyboard = true
			};

			ConcertsChoiceKeyboard = new ReplyKeyboardMarkup
			{
				Keyboard = new[]
				{
					new KeyboardButton[] {PhraseHelper.FutureConcerts, PhraseHelper.PastConcerts},
					new KeyboardButton[] {PhraseHelper.AllConcerts, PhraseHelper.MainMenu}
				},
				ResizeKeyboard = true
			};

			ConcertKeyboard = new ReplyKeyboardMarkup
			{
				Keyboard = new[]
				{
					new KeyboardButton[] {PhraseHelper.Facebook, PhraseHelper.ConcertTickets},
					new KeyboardButton[] {PhraseHelper.Subscribe},
					new KeyboardButton[] {PhraseHelper.ShortDescription, PhraseHelper.FullDescription},
					new KeyboardButton[] {PhraseHelper.Poster, PhraseHelper.Media},
					new KeyboardButton[] {PhraseHelper.BackToConcerts, PhraseHelper.MainMenu}
				},
				ResizeKeyboard = true
			};
		}

		public static async Task<TelegramBotClient> GetBotClientAsync(IServiceProvider serviceProvider)
		{
			if (Client != null)
			{
				return Client;
			}

			ServiceProvider = serviceProvider;

			Client = new TelegramBotClient("723676644:AAE9j7lkkUdGnef3JMerwC6hHXVLkccdyLk");
			await Client.SetWebhookAsync($"{AppLink}/api/message/popcornupdate");
			return Client;
		}

		public static async Task ProcessCallbackMessage(CallbackQuery callback)
		{
			var callBackMessage = callback.Data;
			var message = callback.Message;

			if (callBackMessage.Contains(PhraseHelper.SubscribeCallBack))
			{
				var reply = SubscribeToConcert(message.Chat.Id,
					Convert.ToInt32(callBackMessage.Replace(PhraseHelper.SubscribeCallBack, "")));
				await Client.SendTextMessageAsync(message.Chat.Id, reply,
					replyMarkup: MainKeyboard);
				return;
			}

			message.Text = callBackMessage;
			await ProcessMessage(message);
		}

		public static async Task ProcessMessage(Message message)
		{
			try
			{
				if (message.Text == "/subscribe3")
				{
					await Client.SendTextMessageAsync(message.Chat.Id, "АГА, вот оно!");
					return;
				}

				var chatId = message.Chat.Id;

				var mainInfo = MemoryCacheHelper.GetMainInfo();
				var concerts = MemoryCacheHelper.GetConcerts();

				//concert message checking
				var messageParts = message.Text.Split(" ");
				var potentialConcertTitle = string.Join(" ", messageParts.Take(messageParts.Length - 1));
				var concert =
					concerts.FirstOrDefault(c => c.Artist == message.Text || c.Artist == potentialConcertTitle);
				if (concert != null)
				{
					SetUserConcert(chatId, concert.Id);
					await Client.SendTextMessageAsync(chatId,
						$"{concert.Artist}\r\n{concert.EventDate:dd-MM-yyyy HH:mm} {concert.Venue}\r\n" +
						$"{concert.ShortDescription}",
						replyMarkup: ConcertKeyboard);

					return;
				}

				if (UserCurrentConcert.ContainsKey(chatId))
				{
					concert = concerts.First(c => c.Id == UserCurrentConcert[chatId]);
					switch (message.Text)
					{
						case PhraseHelper.ConcertTickets:
						{
							if (string.IsNullOrEmpty(concert.TicketsLink))
							{
								await Client.SendTextMessageAsync(chatId, "Продажа билетов начнется скоро",
									replyMarkup: ConcertKeyboard);
							}
							else
							{
								await Client.SendTextMessageAsync(chatId, concert.TicketsLink,
									replyMarkup: ConcertKeyboard);
							}

							return;
						}
						case PhraseHelper.Facebook:
						{
							await Client.SendTextMessageAsync(chatId, concert.FacebookLink,
								replyMarkup: ConcertKeyboard);
							return;
						}
						case PhraseHelper.Subscribe:
						{
							var reply = SubscribeToConcert(chatId, concert.Id);
							await Client.SendTextMessageAsync(chatId, reply, replyMarkup: MainKeyboard);
							return;
						}
						case PhraseHelper.ShortDescription:
						{
							await Client.SendTextMessageAsync(chatId, concert.ShortDescription,
								replyMarkup: ConcertKeyboard);
							return;
						}
						case PhraseHelper.FullDescription:
						{
							await Client.SendTextMessageAsync(chatId, concert.FullDescription,
								replyMarkup: ConcertKeyboard);
							return;
						}
						case PhraseHelper.Poster:
						{
							await Client.SendPhotoAsync(chatId, concert.Poster, replyMarkup: ConcertKeyboard);
							return;
						}
						case PhraseHelper.Media:
						{
							await Client.SendTextMessageAsync(chatId, concert.TicketsLink,
								replyMarkup: ConcertKeyboard);
							return;
						}
						case PhraseHelper.BackToConcerts:
						{
							UserCurrentConcert.Remove(chatId);
							message.Text = UserCurrentConcertsType.TryGetValue(chatId, out string concertsType)
								? concertsType
								: PhraseHelper.AllConcerts;
							break;
						}
					}
				}

				if (message.Text == PhraseHelper.Back)
				{
					var userHasState = UserCurrentConcertsType.TryGetValue(chatId, out string concertsType);
					message.Text = userHasState ? concertsType : "/start";
					if (userHasState)
					{
						UserCurrentConcertsType.Remove(chatId);
					}
				}

				switch (message.Text)
				{
					case PhraseHelper.Start:
					case PhraseHelper.MainMenu:
					{
						if (message.Text == PhraseHelper.Start)
						{
							InsertNewUser(message);
						}

						UserCurrentConcert.Remove(chatId);
						UserCurrentConcertsType.Remove(chatId);
						await Client.SendTextMessageAsync(chatId, mainInfo.HelloText, replyMarkup: MainKeyboard);
						return;
					}
					case PhraseHelper.About:
					{
						await Client.SendTextMessageAsync(chatId, mainInfo.AboutText, replyMarkup: MainKeyboard);
						return;
					}
					case PhraseHelper.Contacts:
					{
						await Client.SendTextMessageAsync(chatId, mainInfo.ContactsText, parseMode: ParseMode.Html, replyMarkup: MainKeyboard,
							disableWebPagePreview: true);
						return;
					}
					case PhraseHelper.Concerts:
					case PhraseHelper.BackToConcerts:
					{
						await Client.SendTextMessageAsync(chatId, mainInfo.ConcertsText,
							replyMarkup: ConcertsChoiceKeyboard);
						return;
					}
					case PhraseHelper.FutureConcerts:
					{
						SetUserConcersType(chatId, PhraseHelper.FutureConcerts);
						await Client.SendTextMessageAsync(chatId, mainInfo.ConcertsText,
							replyMarkup: ConcertsKeyboard(MemoryCacheHelper.GetConcerts()
								.Where(c => c.EventDate > DateTime.Now).OrderByDescending(c => c.EventDate).ToList()));
						return;
					}
					case PhraseHelper.PastConcerts:
					{
						SetUserConcersType(chatId, PhraseHelper.PastConcerts);
						await Client.SendTextMessageAsync(chatId, mainInfo.ConcertsText,
							replyMarkup: ConcertsKeyboard(MemoryCacheHelper.GetConcerts()
								.Where(c => c.EventDate <= DateTime.Now).OrderByDescending(c => c.EventDate).ToList()));
						return;
					}
					case PhraseHelper.AllConcerts:
					{
						SetUserConcersType(chatId, PhraseHelper.AllConcerts);
						await Client.SendTextMessageAsync(chatId, mainInfo.ConcertsText,
							replyMarkup: ConcertsKeyboard(MemoryCacheHelper.GetConcerts()
								.OrderByDescending(c => c.EventDate).ToList()));
						return;
					}
					case PhraseHelper.Back:
					{
						UserCurrentConcert.Remove(chatId);
						UserCurrentConcertsType.Remove(chatId);
						await Client.SendTextMessageAsync(chatId, mainInfo.HelloText, replyMarkup: MainKeyboard);
						return;
					}
					case PhraseHelper.Tickets:
					{
						await Client.SendTextMessageAsync(chatId, mainInfo.TicketsText, replyMarkup: MainKeyboard);
						return;
					}
					case PhraseHelper.Sales:
					{
						await Client.SendTextMessageAsync(chatId, mainInfo.SalesText, replyMarkup: MainKeyboard);
						return;
					}
					default:
					{
						await Client.SendTextMessageAsync(chatId, PhraseHelper.InvalidCommand,
							replyMarkup: MainKeyboard);
						return;
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		private static ReplyKeyboardMarkup ConcertsKeyboard(List<Concert> concerts)
		{
			var structured = new List<KeyboardButton[]>();
			var step = 2;
			for (var i = 0; i < concerts.Count; i = i + step)
			{
				structured.Add(concerts.Skip(i).Take(step)
					.Select(c => new KeyboardButton($"{c.Artist} {c.EventDate:MM/dd}")).ToArray());
			}

			structured.Add(new KeyboardButton[] {PhraseHelper.BackToConcerts});

			return new ReplyKeyboardMarkup
			{
				Keyboard = structured.ToArray(),
				ResizeKeyboard = true
			};
		}

		private static void SetUserConcert(long chatId, int concertId)
		{
			if (UserCurrentConcert.ContainsKey(chatId))
			{
				UserCurrentConcert[chatId] = concertId;
			}
			else
			{
				UserCurrentConcert.Add(chatId, concertId);
			}

			using (var serviceScope = ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				var dbContext = serviceScope.ServiceProvider.GetService<PopCornDbContext>();
				var user = dbContext.Users.First(u => u.ChatId == chatId);
				dbContext.ConcertVisit.Add(new ConcertVisit
				{
					UserId = user.Id,
					ConcertId = concertId,
					VisitDate = DateTime.Now
				});
				dbContext.SaveChanges();
			}
		}

		private static void SetUserConcersType(long chatId, string concertsType)
		{
			if (UserCurrentConcertsType.ContainsKey(chatId))
			{
				UserCurrentConcertsType[chatId] = concertsType;
			}
			else
			{
				UserCurrentConcertsType.Add(chatId, concertsType);
			}
		}

		private static string SubscribeToConcert(long chatId, int concertId)
		{
			using (var serviceScope = ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				var dbContext = serviceScope.ServiceProvider.GetService<PopCornDbContext>();
				var user = dbContext.Users.First(u => u.ChatId == chatId);

				if (!dbContext.UserSubscription.Any(s => s.UserId == user.Id && s.ConcertId == concertId))
				{
					dbContext.UserSubscription.Add(new UserSubscription
					{
						UserId = user.Id,
						ConcertId = concertId
					});
					dbContext.SaveChanges();

					return $"{user.FirstName}, теперь вы подписаны на новости концерта";
				}

				return $"{user.FirstName}, вы уже подписаны на новости концерта";
			}
		}

		public static async Task SendNewPostAlert(News post)
		{
			List<User> neededUsers;
			List<long> subscribedUsers;
			bool insertSubscriptionLink;
			using (var serviceScope = ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				var dbContext = serviceScope.ServiceProvider.GetService<PopCornDbContext>();
				insertSubscriptionLink = post.ConcertId.HasValue && post.IsCommonPost;
				neededUsers = post.ConcertId.HasValue && !post.IsCommonPost
					? dbContext.UserSubscription.Include(s => s.User).Where(s => s.ConcertId == post.ConcertId.Value)
						.Select(s => s.User).ToList()
					: dbContext.Users.ToList();
				subscribedUsers = insertSubscriptionLink
					? dbContext.UserSubscription.Include(s => s.User).Where(s => s.ConcertId == post.ConcertId.Value)
						.Select(s => s.User.ChatId).ToList()
					: null;
			}

			var subscriptionLink = insertSubscriptionLink
				? new InlineKeyboardMarkup(new[]
				{
					InlineKeyboardButton.WithCallbackData(PhraseHelper.Subscribe,
						PhraseHelper.SubscribeCallBack + post.ConcertId)
				})
				: null;
			foreach (var user in neededUsers)
			{
				await Client.SendTextMessageAsync(user.ChatId,
					$"{post.Title}\r\n\r\n{post.Desription}\r\n\r\n{post.Link}",
					replyMarkup: insertSubscriptionLink && !subscribedUsers.Contains(user.ChatId)
						? subscriptionLink
						: null);
			}
		}

		private static void InsertNewUser(Message message)
		{
			using (var serviceScope = ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				var dbContext = serviceScope.ServiceProvider.GetService<PopCornDbContext>();
				if (!dbContext.Users.Any(u => u.ChatId == message.Chat.Id))
				{
					dbContext.Add(new User
					{
						ChatId = message.Chat.Id,
						UserId = message.From.Id,
						FirstName = message.From.FirstName,
						LastName = message.From.LastName
					});
					dbContext.SaveChanges();
				}
			}
		}
	}
}