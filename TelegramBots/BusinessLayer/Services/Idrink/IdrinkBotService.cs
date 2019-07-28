using System;
using System.Collections.Generic;
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
		private readonly IdrinkDbContext _repository;
		public static string[][] MainKeyboard;
		public static string[][] SettingsKeyboard;
		public static string[][] HistoryKeyboard;
		public static HashSet<int> SubscribedToList = new HashSet<int>();
		public static HashSet<int> MySubscribersList = new HashSet<int>();
		public static HashSet<int> PhotoStepList = new HashSet<int>();
		public static TelegramBotClient Client;

		static IdrinkBotService()
		{
			MainKeyboard = new[]
			{
				new[] {PhraseHelper.Idrink},
				new[] {PhraseHelper.DrinkHistory, PhraseHelper.SubscribeToFriend},
				new[] {PhraseHelper.Settings, PhraseHelper.AboutBot}
			};

			SettingsKeyboard = new[]
			{
				new[] {PhraseHelper.SubscribedToList, PhraseHelper.MySubscribersList},
				new[] {PhraseHelper.MainMenu}
			};

			HistoryKeyboard = new[]
			{
				new[] {PhraseHelper.LastWeek, PhraseHelper.LastMonth},
				new[] {PhraseHelper.MainMenu}
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
				Client = new TelegramBotClient(ConfigData.TelegramKey);
			}
		}

		public static async Task SetWebHook()
		{
			await Client.DeleteWebhookAsync();
			await Client.SetWebhookAsync($"{ConfigData.AppLink}/api/message/idrinkupdate");
		}

		public async Task<Message> SendTextMessage(AnswerMessageBase message)
		{
			return await Client.SendTextMessage(message);
		}

		public async Task ProcessCallbackMessage(CallbackQuery callback)
		{
			var message = callback.Message;
			try
			{
				if (callback.Data.Contains(PhraseHelper.SubscribeToCode))
				{
					var subscribeToId = Convert.ToInt32(callback.Data.Replace(PhraseHelper.SubscribeToCode, ""));
					var subscribeTo = _repository.Users.First(u => u.Id == subscribeToId);
					var currentUser = _repository.Users.First(u => u.ChatId == message.Chat.Id);

					if (_repository.Subscriptions
						.Any(s => s.SubscriberId == currentUser.Id && s.SubscribedOn.ChatId == subscribeToId))
					{
						await SendTextMessage(new AnswerMessageBase(message.Chat.Id, PhraseHelper.AlreadySubscribed,
							MainKeyboard));
						return;
					}

					_repository.Subscriptions.Add(new Subscription
					{
						SubscriberId = currentUser.Id,
						SubscribedOnId = subscribeToId
					});
					_repository.SaveChanges();

					await SendTextMessage(new AnswerMessageBase(message.Chat.Id,
						string.Format(PhraseHelper.SuccessfullySubscribe, subscribeTo.FirstName, subscribeTo.LastName),
						MainKeyboard));

					await Client.DeleteMessageAsync(message.Chat.Id, message.MessageId);

					await SendTextMessage(new AnswerMessageBase(subscribeTo.ChatId,
						string.Format(PhraseHelper.YouHaveNewSubscriber, currentUser.FirstName, currentUser.LastName,
							string.IsNullOrEmpty(currentUser.UserName) ? "" : $" (@{currentUser.UserName})")));

					return;
				}

				if (callback.Data.Contains(PhraseHelper.YesCode))
				{
					var currentUser = _repository.Users.First(u => u.ChatId == message.Chat.Id);
					var drinkingId = Convert.ToInt32(callback.Data.Replace(PhraseHelper.YesCode, ""));
					var drinking = _repository.Users.First(u => u.Id == drinkingId);

					await Client.DeleteMessageAsync(message.Chat.Id, message.MessageId);
					await SendTextMessage(new AnswerMessageBase(drinking.ChatId,
						string.Format(PhraseHelper.InterestedToDrinkWithYou, currentUser.FirstName,
							currentUser.LastName,
							string.IsNullOrEmpty(currentUser.UserName) ? "" : $" (@{currentUser.UserName})")));
					return;
				}

				if (callback.Data == PhraseHelper.NoCode)
				{
					await Client.DeleteMessageAsync(message.Chat.Id, message.MessageId);
					return;
				}
			}
			catch (Exception ex)
			{
				_repository.Add(new Log
				{
					ChatId = message.Chat.Id,
					LogDate = DateTime.Now,
					Message = ex.Message,
					StackTrace = ex.StackTrace
				});
				_repository.SaveChanges();
			}

			await ProcessMessageBase(message, callback.Data);
		}

		public async Task ProcessMessage(Message message)
		{
			await ProcessMessageBase(message, message.Text);
		}

		public async Task ProcessMessageBase(Message message, string messageText)
		{
			long errorUserId = 0;
			try
			{
				if (DateTime.UtcNow.Subtract(message.Date).TotalMinutes > 3)
				{
					return;
				}

				var chatId = message.Chat.Id;
				errorUserId = chatId;
				var userFirstName = message.Chat.FirstName;
				var userLastName = message.Chat.LastName;
				var userName = message.Chat.Username;
				var userId = InsertNewUser(chatId, userFirstName, userLastName, userName);
				errorUserId = userId;

				if (PhotoStepList.Contains(userId))
				{
					var drinkingRecords = _repository.DrinkHistory.Where(h => h.User.ChatId == chatId)
						.OrderByDescending(h => h.DrinkTime).Take(2).ToList();
					var currentDrinking = drinkingRecords[0];
					var prevDrinking = drinkingRecords.Count == 1 ? null : drinkingRecords[1];
					await SendTextMessage(new AnswerMessageBase(chatId,
						prevDrinking == null
							? PhraseHelper.IdrinkCongrats
							: string.Format(PhraseHelper.IdrinkCongratsWithDate,
								Math.Floor((currentDrinking.DrinkTime - prevDrinking.DrinkTime).TotalDays),
								(currentDrinking.DrinkTime - prevDrinking.DrinkTime).Hours,
								(currentDrinking.DrinkTime - prevDrinking.DrinkTime).Minutes),
						MainKeyboard));
					
					var subscribers = _repository.Subscriptions.Include(s => s.Subscriber)
						.Where(s => s.SubscribedOn.ChatId == chatId).ToList();
					foreach (var subscriber in subscribers)
					{
						try
						{
							errorUserId = subscriber.Subscriber.Id;
							var subscriberChatId = subscriber.Subscriber.ChatId;
							await SendTextMessage(new AnswerMessageBase(subscriberChatId,
								string.Format(PhraseHelper.DrinkingNow, userFirstName, userLastName,
									string.IsNullOrEmpty(userName) ? "" : $" (@{userName})"),
								MainKeyboard));
							if (currentDrinking.Latitude.HasValue && currentDrinking.Longitude.HasValue)
							{
								await Client.SendLocationAsync(subscriberChatId, currentDrinking.Latitude.Value,
									currentDrinking.Longitude.Value);
							}

							if (message.Type == MessageType.Photo)
							{
								var photoId = message.Photo.Last().FileId;
								var photoInfo = await Client.GetFileAsync(photoId);
								var webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(
									$"https://api.telegram.org/file/bot{ConfigData.TelegramKey}/{photoInfo.FilePath}");
								webRequest.AllowWriteStreamBuffering = true;
								var webResponse = webRequest.GetResponse();
								var stream = webResponse.GetResponseStream();
								await Client.SendPhotoAsync(subscriberChatId, stream);
							}

							await SendTextMessage(
								new AnswerMessageBase(subscriberChatId, PhraseHelper.AreYouInterested)
								{
									InlineKeyboard = new Dictionary<string, string>
									{
										{PhraseHelper.Yes, $"{PhraseHelper.YesCode}{userId}"},
										{PhraseHelper.No, $"{PhraseHelper.NoCode}"}
									}
								});
						}
						catch (Exception ex)
						{
							_repository.Add(new Log
							{
								ChatId = errorUserId,
								LogDate = DateTime.Now,
								Message = ex.Message,
								StackTrace = ex.StackTrace
							});
							_repository.SaveChanges();
						}
					}

					PhotoStepList.Remove(userId);

					return;
				}

				if (message.Type == MessageType.Contact)
				{
					SubscribedToList.Remove(userId);
					MySubscribersList.Remove(userId);
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
							string.Format(PhraseHelper.ContactDoesntUseBot, contact.FirstName, contact.LastName),
							MainKeyboard));
						return;
					}

					_repository.Subscriptions.Add(new Subscription
					{
						SubscriberId = userId,
						SubscribedOnId = user.Id
					});
					_repository.SaveChanges();

					await SendTextMessage(new AnswerMessageBase(chatId,
						string.Format(PhraseHelper.SuccessfullySubscribe, contact.FirstName, contact.LastName),
						MainKeyboard));

					errorUserId = user.Id;
					if (_repository.Subscriptions
						.Any(s => s.Subscriber.ChatId == user.ChatId && s.SubscribedOn.ChatId == chatId))
					{
						await SendTextMessage(new AnswerMessageBase(user.ChatId,
							string.Format(PhraseHelper.YouHaveNewSubscriber, userFirstName, userLastName,
								string.IsNullOrEmpty(userName) ? "" : $" (@{userName})")));
					}
					else
					{
						await SendTextMessage(new AnswerMessageBase(user.ChatId,
							string.Format(PhraseHelper.YouHaveNewSubscriber, userFirstName, userLastName,
								string.IsNullOrEmpty(userName) ? "" : $" (@{userName})"))
						{
							InlineKeyboard = new Dictionary<string, string>
							{
								{PhraseHelper.SubscribeTo, $"{PhraseHelper.SubscribeToCode}{userId}"}
							}
						});
					}

					return;
				}

				if (message.Type == MessageType.Location)
				{
					SubscribedToList.Remove(userId);
					MySubscribersList.Remove(userId);
					messageText = PhraseHelper.Location;
				}

				if (string.IsNullOrEmpty(messageText))
				{
					return;
				}

				errorUserId = userId;
				switch (messageText)
				{
					case PhraseHelper.Start:
					case PhraseHelper.AboutBot:
					{
						await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.AboutBotText,
							MainKeyboard));
						return;
					}
					case PhraseHelper.MainMenu:
					{
						await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.MainMenuText,
							MainKeyboard));
						return;
					}
					case PhraseHelper.Idrink:
					{
						var lastDrinkTime = _repository.DrinkHistory.Where(h => h.UserId == userId)
							.OrderByDescending(h => h.DrinkTime).FirstOrDefault()?.DrinkTime;
						if (lastDrinkTime.HasValue && DateTime.Now.Subtract(lastDrinkTime.Value).TotalMinutes < 30)
						{
							await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.YouDrinkTooMuch,
								MainKeyboard));
							return;
						}

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
						_repository.Add(new DrinkHistory
						{
							UserId = userId,
							DrinkTime = DateTime.Now,
							Latitude = messageText == PhraseHelper.Location ? message.Location?.Latitude : null,
							Longitude = messageText == PhraseHelper.Location ? message.Location?.Longitude : null
						});
						_repository.SaveChanges();

						PhotoStepList.Add(userId);

						await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.WouldYouLikeToAddPhoto,
							new[] {new[] {PhraseHelper.Skip}}));

						return;
					}
					case PhraseHelper.DrinkHistory:
					{
						await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.DrinkHistoryQuestion,
							HistoryKeyboard));
						return;
					}
					case PhraseHelper.LastWeek:
					case PhraseHelper.LastMonth:
					{
						var dateLimit = messageText == PhraseHelper.LastWeek
							? DateTime.Now.AddDays(-7).Date
							: DateTime.Now.AddMonths(-1).Date;
						var data = _repository.DrinkHistory
							.Where(h => h.User.ChatId == chatId && h.DrinkTime >= dateLimit)
							.OrderBy(h => h.DrinkTime).ToList();

						foreach (var drink in data)
						{
							if (!drink.Latitude.HasValue || !drink.Longitude.HasValue)
							{
								await SendTextMessage(new AnswerMessageBase(chatId,
									string.Format(PhraseHelper.YouDrinkAt,
										drink.DrinkTime.ToString("dd-MM-yyyy HH:mm")), HistoryKeyboard));
							}
							else
							{
								await SendTextMessage(new AnswerMessageBase(chatId,
									string.Format(PhraseHelper.YouDrinkAt,
										drink.DrinkTime.ToString("dd-MM-yyyy HH:mm")), HistoryKeyboard));
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
					case PhraseHelper.Settings:
					{
						await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.ManageYourSettings,
							SettingsKeyboard));
						return;
					}
					case PhraseHelper.SubscribedToList:
					{
						SubscribedToList.Add(userId);
						var result = _repository.Subscriptions.Where(s => s.Subscriber.ChatId == chatId)
							.OrderBy(s => s.SubscribedOn.FirstName).ThenBy(s => s.SubscribedOn.LastName)
							.Select(s =>
								$"{s.SubscribedOn.Id} - {s.SubscribedOn.FirstName} {s.SubscribedOn.LastName}" +
								(string.IsNullOrEmpty(s.SubscribedOn.UserName) ? "" : $" @{s.SubscribedOn.UserName}"))
							.ToList();
						await SendTextMessage(new AnswerMessageBase(chatId,
							result.Count == 0
								? PhraseHelper.YouDontSubscriberToAnyone
								: $"{string.Join("\r\n", result)}\r\n\r\n{PhraseHelper.ForUnsubscribe}",
							SettingsKeyboard));
						return;
					}
					case PhraseHelper.MySubscribersList:
					{
						MySubscribersList.Add(userId);
						var result = _repository.Subscriptions.Where(s => s.SubscribedOn.ChatId == chatId)
							.OrderBy(s => s.Subscriber.FirstName).ThenBy(s => s.Subscriber.LastName)
							.Select(s =>
								$"{s.Subscriber.Id} - {s.Subscriber.FirstName} {s.Subscriber.LastName}" +
								(string.IsNullOrEmpty(s.Subscriber.UserName) ? "" : $" @{s.Subscriber.UserName}"))
							.ToList();
						await SendTextMessage(new AnswerMessageBase(chatId,
							result.Count == 0
								? PhraseHelper.NoSubscribers
								: $"{string.Join("\r\n", result)}\r\n\r\n{PhraseHelper.ForUnsubscribeFromMe}",
							SettingsKeyboard));
						return;
					}
					default:
					{
						if (SubscribedToList.Contains(userId) && int.TryParse(messageText, out var number))
						{
							var subscription = _repository.Subscriptions.FirstOrDefault(s =>
								s.SubscriberId == userId && s.SubscribedOnId == number);
							if (subscription == null)
							{
								await SendTextMessage(new AnswerMessageBase(chatId,
									PhraseHelper.YouAreNowSubscribed,
									SettingsKeyboard));
								return;
							}

							_repository.Remove(subscription);
							_repository.SaveChanges();

							await SendTextMessage(new AnswerMessageBase(chatId,
								PhraseHelper.SuccessfullyUnSubscribe,
								SettingsKeyboard));

							SubscribedToList.Remove(userId);

							return;
						}

						if (MySubscribersList.Contains(userId) && int.TryParse(messageText, out number))
						{
							await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.InvalidCommand,
								SettingsKeyboard));
							var subscription = _repository.Subscriptions.FirstOrDefault(s =>
								s.SubscriberId == number && s.SubscribedOnId == userId);
							if (subscription == null)
							{
								await SendTextMessage(new AnswerMessageBase(chatId,
									PhraseHelper.ThisUserNotSubscribedOnYou,
									SettingsKeyboard));
								return;
							}

							_repository.Remove(subscription);
							_repository.SaveChanges();

							await SendTextMessage(new AnswerMessageBase(chatId,
								PhraseHelper.SuccessfullyRemoveSubscriber,
								SettingsKeyboard));

							MySubscribersList.Remove(userId);

							return;
						}

						await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.InvalidCommand, MainKeyboard));
						return;
					}
				}
			}
			catch (Exception ex)
			{
				_repository.Add(new Log
				{
					ChatId = errorUserId,
					LogDate = DateTime.Now,
					Message = ex.Message,
					StackTrace = ex.StackTrace
				});
				_repository.SaveChanges();
			}
		}

		public async Task<string> SendGlobalMessageWithDateCondition(IdrinkMessage message)
		{
			var neededUsers = message.IsGlobal
				? _repository.Users.Select(u => new {u.Id, u.ChatId}).ToList()
				: _repository.Users
					.Where(u => (message.Users == null || message.Users.Contains(u.Id)) && (message.IsDrank
						            ? u.DrinkHistory.Any(h => h.DrinkTime >= message.DateCondition)
						            : !u.DrinkHistory.Any(h => h.DrinkTime >= message.DateCondition)))
					.Select(u => new {u.Id, u.ChatId}).ToList();
			foreach (var user in neededUsers)
			{
				try
				{
					await SendTextMessage(new AnswerMessageBase(user.ChatId, message.Body));
				}
				catch (Exception ex)
				{
					_repository.Add(new Log
					{
						ChatId = user.ChatId,
						LogDate = DateTime.Now,
						Message = ex.Message,
						StackTrace = ex.StackTrace
					});
					_repository.SaveChanges();
				}
			}

			return string.Join(",", neededUsers.Select(u => u.Id));
		}

		private int InsertNewUser(long chatId, string userFirstName, string userLastName, string userName)
		{
			var user = _repository.Users.FirstOrDefault(u => u.ChatId == chatId);
			if (user == null)
			{
				user = new User
				{
					ChatId = chatId,
					FirstName = userFirstName,
					LastName = userLastName,
					UserName = userName
				};
				_repository.Add(user);
				_repository.SaveChanges();
			}
			else if (string.IsNullOrEmpty(user.UserName) && !string.IsNullOrEmpty(userName))
			{
				user.UserName = userName;
				_repository.SaveChanges();
			}

			return user.Id;
		}
	}
}