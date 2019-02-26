﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TelegramBots.Context;
using TelegramBots.Helpers;
using TelegramBots.Models;

namespace TelegramBots.Services
{
	public class PopCornBotServiceBase
	{
		public static IServiceProvider ServiceProvider;
		public static string[][] MainKeyboard;
		public static string[][] ConcertsChoiceKeyboard;
		public static string[][] ConcertKeyboard;
		public static Dictionary<string, int> UserCurrentConcert = new Dictionary<string, int>();
		public static Dictionary<string, string> UserCurrentConcertsType = new Dictionary<string, string>();

		static PopCornBotServiceBase()
		{
			MainKeyboard = new[]
			{
				new[] {PhraseHelper.About, PhraseHelper.Contacts},
				new[] {PhraseHelper.Tickets},
				new[] {PhraseHelper.Concerts},
				new[] {PhraseHelper.Sales}
			};

			ConcertsChoiceKeyboard = new[]
			{
				new[] {PhraseHelper.FutureConcerts, PhraseHelper.PastConcerts},
				new[] {PhraseHelper.AllConcerts, PhraseHelper.MainMenu}
			};

			ConcertKeyboard = new[]
			{
				new[] {PhraseHelper.Facebook, PhraseHelper.ConcertTickets},
				new[] {PhraseHelper.Subscribe},
				new[] {PhraseHelper.ShortDescription, PhraseHelper.FullDescription},
				new[] {PhraseHelper.Poster, PhraseHelper.Media},
				new[] {PhraseHelper.BackToConcerts, PhraseHelper.MainMenu}
			};
		}

		public static string ProcessCallbackMessageBase(string callBackMessage, long chatId, int messageId,
			string messageText)
		{
			if (callBackMessage.Contains(PhraseHelper.SubscribeCallBack))
			{
				return SubscribeToConcert(chatId.ToString(),
					Convert.ToInt32(callBackMessage.Replace(PhraseHelper.SubscribeCallBack, "")));
			}

			return null;
		}

		public virtual Task SendTextMessage(AnswerMessageBase message)
		{
			return Task.FromResult(default(object));
		}

		public async Task ProcessMessageBase(string chatId, string userFirstName, string userLastName,
			string messageText)
		{
			try
			{
				var mainInfo = MemoryCacheHelper.GetMainInfo();
				var concerts = MemoryCacheHelper.GetConcerts();

				//concert message checking
				var messageParts = messageText.Split(" ");
				var potentialConcertTitle = string.Join(" ", messageParts.Take(messageParts.Length - 1));
				var concert =
					concerts.FirstOrDefault(c => c.Artist == messageText || c.Artist == potentialConcertTitle);
				if (concert != null)
				{
					SetUserConcert(chatId, concert.Id);
					await SendTextMessage(new AnswerMessageBase(chatId,
						$"{concert.Artist}\r\n{concert.EventDate:dd-MM-yyyy HH:mm} {concert.Venue}\r\n" +
						$"{concert.ShortDescription}", ConcertKeyboard));

					return;
				}

				if (UserCurrentConcert.ContainsKey(chatId))
				{
					concert = concerts.First(c => c.Id == UserCurrentConcert[chatId]);
					switch (messageText)
					{
						case PhraseHelper.ConcertTickets:
						{
							if (string.IsNullOrEmpty(concert.TicketsLink))
							{
								await SendTextMessage(new AnswerMessageBase(chatId, "Продажа билетов начнется скоро",
									ConcertKeyboard));
							}
							else
							{
								await SendTextMessage(new AnswerMessageBase(chatId, concert.TicketsLink,
									ConcertKeyboard));
							}

							return;
						}
						case PhraseHelper.Facebook:
						{
							await SendTextMessage(new AnswerMessageBase(chatId, concert.FacebookLink, ConcertKeyboard));
							return;
						}
						case PhraseHelper.Subscribe:
						{
							var reply = SubscribeToConcert(chatId, concert.Id);
							await SendTextMessage(new AnswerMessageBase(chatId, reply, MainKeyboard));
							return;
						}
						case PhraseHelper.ShortDescription:
						{
							await SendTextMessage(new AnswerMessageBase(chatId, concert.ShortDescription,
								ConcertKeyboard));
							return;
						}
						case PhraseHelper.FullDescription:
						{
							await SendTextMessage(new AnswerMessageBase(chatId, concert.FullDescription,
								ConcertKeyboard));
							return;
						}
						case PhraseHelper.Poster:
						{
							await SendTextMessage(new AnswerMessageBase(chatId)
							{
								Photo = concert.Poster,
								Keyboard = ConcertKeyboard
							});
							return;
						}
						case PhraseHelper.Media:
						{
							await SendTextMessage(new AnswerMessageBase(chatId, concert.TicketsLink, ConcertKeyboard));
							return;
						}
						case PhraseHelper.BackToConcerts:
						{
							UserCurrentConcert.Remove(chatId);
							messageText = UserCurrentConcertsType.TryGetValue(chatId, out string concertsType)
								? concertsType
								: PhraseHelper.AllConcerts;
							break;
						}
					}
				}

				if (messageText == PhraseHelper.Back)
				{
					var userHasState = UserCurrentConcertsType.TryGetValue(chatId, out var concertsType);
					messageText = userHasState ? concertsType : "/start";
					if (userHasState)
					{
						UserCurrentConcertsType.Remove(chatId);
					}
				}

				switch (messageText)
				{
					case PhraseHelper.Start:
					case PhraseHelper.MainMenu:
					{
						if (messageText == PhraseHelper.Start)
						{
							InsertNewUser(chatId, userFirstName, userLastName);
						}

						UserCurrentConcert.Remove(chatId);
						UserCurrentConcertsType.Remove(chatId);
						await SendTextMessage(new AnswerMessageBase(chatId, mainInfo.HelloText, MainKeyboard));
						return;
					}
					case PhraseHelper.About:
					{
						await SendTextMessage(new AnswerMessageBase(chatId, mainInfo.AboutText, MainKeyboard));
						return;
					}
					case PhraseHelper.Contacts:
					{
						await SendTextMessage(new AnswerMessageBase(chatId, mainInfo.ContactsText, MainKeyboard)
						{
							IsHtml = true
						});
						return;
					}
					case PhraseHelper.Concerts:
					case PhraseHelper.BackToConcerts:
					{
						await SendTextMessage(new AnswerMessageBase(chatId, mainInfo.ConcertsText,
							ConcertsChoiceKeyboard));
						return;
					}
					case PhraseHelper.FutureConcerts:
					{
						SetUserConcersType(chatId, PhraseHelper.FutureConcerts);
						await SendTextMessage(new AnswerMessageBase(chatId, mainInfo.ConcertsText,
							ConcertsKeyboard(MemoryCacheHelper.GetConcerts()
								.Where(c => c.EventDate > DateTime.Now).OrderByDescending(c => c.EventDate).ToList())));
						return;
					}
					case PhraseHelper.PastConcerts:
					{
						SetUserConcersType(chatId, PhraseHelper.PastConcerts);
						await SendTextMessage(new AnswerMessageBase(chatId, mainInfo.ConcertsText,
							ConcertsKeyboard(MemoryCacheHelper.GetConcerts()
								.Where(c => c.EventDate <= DateTime.Now).OrderByDescending(c => c.EventDate)
								.ToList())));
						return;
					}
					case PhraseHelper.AllConcerts:
					{
						SetUserConcersType(chatId, PhraseHelper.AllConcerts);
						await SendTextMessage(new AnswerMessageBase(chatId, mainInfo.ConcertsText,
							ConcertsKeyboard(MemoryCacheHelper.GetConcerts()
								.OrderByDescending(c => c.EventDate).ToList())));
						return;
					}
					case PhraseHelper.Back:
					{
						UserCurrentConcert.Remove(chatId);
						UserCurrentConcertsType.Remove(chatId);
						await SendTextMessage(new AnswerMessageBase(chatId, mainInfo.HelloText, MainKeyboard));
						return;
					}
					case PhraseHelper.Tickets:
					{
						await SendTextMessage(new AnswerMessageBase(chatId, mainInfo.TicketsText, MainKeyboard));
						return;
					}
					case PhraseHelper.Sales:
					{
						await SendTextMessage(new AnswerMessageBase(chatId, mainInfo.SalesText, MainKeyboard));
						return;
					}
					default:
					{
						await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.InvalidCommand, MainKeyboard));
						return;
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		private static string SubscribeToConcert(string chatId, int concertId)
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


		private static void SetUserConcert(string chatId, int concertId)
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

		private static void SetUserConcersType(string chatId, string concertsType)
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

		private static string[][] ConcertsKeyboard(List<Concert> concerts)
		{
			var structured = new List<string[]>();
			var step = 2;
			for (var i = 0; i < concerts.Count; i = i + step)
			{
				structured.Add(concerts.Skip(i).Take(step)
					.Select(c => new string($"{c.Artist} {c.EventDate:MM/dd}")).ToArray());
			}

			structured.Add(new[] {PhraseHelper.BackToConcerts});

			return structured.ToArray();
		}

		private static void InsertNewUser(string chatId, string userFirstName, string userLastName)
		{
			using (var serviceScope = ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				var dbContext = serviceScope.ServiceProvider.GetService<PopCornDbContext>();
				if (!dbContext.Users.Any(u => u.ChatId == chatId))
				{
					dbContext.Add(new User
					{
						ChatId = chatId,
						FirstName = userFirstName,
						LastName = userLastName
					});
					dbContext.SaveChanges();
				}
			}
		}

		public async Task SendNewPostAlert(Post post)
		{
			List<User> neededUsers;
			List<string> subscribedUsers;
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
				? new Dictionary<string, string>
				{
					{
						PhraseHelper.Subscribe, PhraseHelper.SubscribeCallBack + post.ConcertId
					}
				}
				: null;
			foreach (var user in neededUsers)
			{
				await SendTextMessage(
					new AnswerMessageBase(user.ChatId, $"{post.Title}\r\n\r\n{post.Desription}\r\n\r\n{post.Link}")
					{
						InlineKeyboard = insertSubscriptionLink && !subscribedUsers.Contains(user.ChatId)
							? subscriptionLink
							: null
					});
			}
		}
	}
}