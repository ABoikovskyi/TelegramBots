using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Helpers;
using DataLayer.Context;
using DataLayer.Models.DTO;
using DataLayer.Models.Idrink;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DataLayer.Models.Idrink.User;

namespace BusinessLayer.Services.Idrink
{
	public class IdrinkBotService
	{
		public static readonly CultureInfo CurrentCultureInfo = new CultureInfo("ru");
		private readonly IdrinkDbContext _repository;
		public static string[][] MainKeyboard;
		public static TelegramBotClient Client;

		static IdrinkBotService()
		{
			MainKeyboard = new[]
			{
				new[] {PhraseHelper.Idrink},
				new[] {PhraseHelper.DrinkHistory},
				new[] {PhraseHelper.SubscribeToFriend}
			};
		}

		public IdrinkBotService(IdrinkDbContext repository)
		{
			_repository = repository;
		}

		public static void Init()
		{
			if (Client == null)
			{
				Client = new TelegramBotClient(Links.TelegramKey);
			}
		}

		public static async Task SetWebHook()
		{
			await Client.DeleteWebhookAsync();
			await Client.SetWebhookAsync($"{Links.AppLink}/api/message/idrinkupdate");
		}

		public async Task SendTextMessage(AnswerMessageBase message)
		{
			await Client.SendTextMessage(message);
		}
		
		public async Task ProcessCallbackMessage(CallbackQuery callback)
		{
			var message = callback.Message;

			await ProcessMessageBase(message,callback.Data);
		}

		public async Task ProcessMessage(Message message)
		{
			await ProcessMessageBase(message,message.Text);
		}

		public async Task ProcessMessageBase(Message message, string messageText)
		{
			var chatId = message.Chat.Id;
			var userFirstName = message.Chat.FirstName;
			var userLastName = message.Chat.LastName;
			try
			{
				if (message.Type == MessageType.Contact)
				{
					var contact = message.Contact;
					if (chatId == contact.UserId)
					{
						await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.SubscribeToYouself,
							MainKeyboard));
						return;
					}

					if (_repository.Subscriptions
						.Any(s => s.Subscriber.ChatId == chatId && s.SubscribedOn.ChatId == contact.UserId))
					{
						await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.AlreadySubscribed,
							MainKeyboard));
						return;
					}

					var user = _repository.Users.FirstOrDefault(u => u.ChatId == contact.UserId);
					if (user == null)
					{
						await SendTextMessage(new AnswerMessageBase(chatId,
							string.Format(PhraseHelper.ContactDoesntUseBot, contact.FirstName,contact.LastName),
							MainKeyboard));
						return;
					}

					var currentUserId = _repository.Users.First(u => u.ChatId == chatId).Id;
					_repository.Subscriptions.Add(new Subscription
					{
						SubscriberId = currentUserId,
						SubscribedOnId = user.Id
					});
					_repository.SaveChanges();

					await SendTextMessage(new AnswerMessageBase(chatId,
						string.Format(PhraseHelper.SuccessfullySubscribe, contact.FirstName, contact.LastName),
						MainKeyboard));

					await SendTextMessage(new AnswerMessageBase(user.ChatId,
						string.Format(PhraseHelper.YouHaveNewSubscriber, userFirstName, userLastName),
						MainKeyboard));
					return;
				}

				if (message.Type == MessageType.Location)
				{
					messageText = PhraseHelper.Location;
				}

				if (DateTime.TryParse(messageText, CurrentCultureInfo, DateTimeStyles.None, out var drinkingDay))
				{
					messageText = PhraseHelper.CustomDay;
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

						await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.IdrinkHelloText,
							MainKeyboard));
						return;
					}
					case PhraseHelper.Idrink:
					{
						await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.SetGeolocationQuestion,
							new[]
							{
								new[] {PhraseHelper.SetGeolocation, PhraseHelper.NoLocation},
								new[] {PhraseHelper.MainMenu}
							}));
						return;
					}
					case PhraseHelper.Location:
					case PhraseHelper.SetGeolocation:
					case PhraseHelper.NoLocation:
					{
						var lastDrink = _repository.DrinkHistory.Where(h => h.User.ChatId == chatId)
							.OrderByDescending(h => h.DrinkTime).FirstOrDefault()?.DrinkTime;
						var currentDate = DateTime.Now;

						var userId = _repository.Users.First(u => u.ChatId == chatId).Id;

						_repository.Add(new DrinkHistory
						{
							UserId = userId,
							DrinkTime = currentDate,
							Latitude = messageText == PhraseHelper.Location? message.Location?.Latitude : null,
							Longitude = messageText == PhraseHelper.Location ? message.Location?.Longitude : null
						});
						_repository.SaveChanges();

						await SendTextMessage(new AnswerMessageBase(chatId,
							lastDrink == null
								? PhraseHelper.IdrinkCongrats
								: string.Format(PhraseHelper.IdrinkCongratsWithDate,
									Math.Floor((currentDate - lastDrink.Value).TotalDays),
									(currentDate - lastDrink.Value).Hours, (currentDate - lastDrink.Value).Minutes),
							MainKeyboard));

						var subscribers = _repository.Subscriptions.Include(s => s.Subscriber)
							.Where(s => s.SubscribedOn.ChatId == chatId).ToList();
						foreach (var subscriber in subscribers)
						{
							await SendTextMessage(new AnswerMessageBase(subscriber.Subscriber.ChatId,
								string.Format(PhraseHelper.DrinkingNow, userFirstName, userLastName), MainKeyboard));
							var latitude = message.Location?.Latitude;
							if (latitude.HasValue)
							{
								await Client.SendLocationAsync(subscriber.Subscriber.ChatId, latitude.Value, message.Location.Longitude);
							}
						}

						return;
					}
					case PhraseHelper.DrinkHistory:
					{
						await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.DrinkHistoryQuestion,
							new[]
							{
								new[] {PhraseHelper.LastWeek, PhraseHelper.LastMonth},
								new[] {PhraseHelper.MainMenu}
							}));
						return;
					}
					case PhraseHelper.LastWeek:
					case PhraseHelper.LastMonth:
					case PhraseHelper.CustomDay:
					{
						List<DrinkHistory> data;
						if (drinkingDay != DateTime.MinValue)
						{
							data = _repository.DrinkHistory
								.Where(h => h.User.ChatId == chatId && h.DrinkTime.Date == drinkingDay.Date).ToList();
						}
						else
						{
							var dateLimit = messageText == PhraseHelper.LastWeek
								? DateTime.Now.AddDays(-7).Date
								: DateTime.Now.AddMonths(-1).Date;
							data = _repository.DrinkHistory
								.Where(h => h.User.ChatId == chatId && h.DrinkTime >= dateLimit).ToList();
						}

						foreach (var drink in data)
						{
							var latitude = drink.Latitude;
							if (!latitude.HasValue)
							{
								await SendTextMessage(new AnswerMessageBase(chatId,
									string.Format(PhraseHelper.YouDrinkAt,
										drink.DrinkTime.ToString("dd-MM-yyyy HH:mm")), MainKeyboard));
							}
							else
							{
								await SendTextMessage(new AnswerMessageBase(chatId,
									string.Format(PhraseHelper.YouDrinkAt,
										drink.DrinkTime.ToString("dd-MM-yyyy HH:mm")), MainKeyboard));
								await Client.SendLocationAsync(chatId, drink.Latitude.Value, drink.Longitude.Value);
							}
						}

						return;
					}
					case PhraseHelper.SubscribeToFriend:
					{
						await SendTextMessage(new AnswerMessageBase(chatId,
							PhraseHelper.HowToSubscribeToFriend, MainKeyboard));
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

		private int InsertNewUser(long chatId, string userFirstName, string userLastName)
		{
			if (_repository.Users.Any(u => u.ChatId == chatId))
			{
				return 0;
			}

			var user = new User
			{
				ChatId = chatId,
				FirstName = userFirstName,
				LastName = userLastName
			};
			_repository.Add(user);
			_repository.SaveChanges();

			return user.Id;
		}
	}
}