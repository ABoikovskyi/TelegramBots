using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Helpers;
using DataLayer.Models;
using DataLayer.Models.Enums;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBots.Context;
using Game = DataLayer.Models.Enums.Game;

namespace TelegramBots.Services
{
	public class PlayZoneBotService
	{
		public static TelegramBotClient Client;
		public const string AppLink = "https://playzone.ua";
		public static Dictionary<long, UserRequest> RequestsData = new Dictionary<long, UserRequest>();
		public static IServiceProvider ServiceProvider;
		public static Dictionary<string, PlayZone> PlayZones;
		public static Dictionary<string, NumberOfPeople> NumberOfPeoples;
		public static Dictionary<string, GameConsole> GameConsoles;
		public static Dictionary<string, Game> Games;
		public static Dictionary<string, RequestDate> RequestDates;
		public static Dictionary<string, RequestTime> RequestTimes;

		static PlayZoneBotService()
		{
			PlayZones = Enum.GetValues(typeof(PlayZone)).Cast<PlayZone>()
				.ToDictionary(s => s.GetDisplayName(), s => s);
			NumberOfPeoples = Enum.GetValues(typeof(NumberOfPeople)).Cast<NumberOfPeople>()
				.ToDictionary(s => s.GetDisplayName(), s => s);
			GameConsoles = Enum.GetValues(typeof(GameConsole)).Cast<GameConsole>()
				.ToDictionary(s => s.GetDisplayName(), s => s);
			Games = Enum.GetValues(typeof(Game)).Cast<Game>().ToDictionary(s => s.GetDisplayName(), s => s);
			RequestDates = Enum.GetValues(typeof(RequestDate)).Cast<RequestDate>()
				.ToDictionary(s => s.GetDisplayName(), s => s);
			RequestTimes = Enum.GetValues(typeof(RequestTime)).Cast<RequestTime>()
				.ToDictionary(s => s.GetDisplayName(), s => s);
		}

		public static async Task<TelegramBotClient> GetBotClientAsync(IServiceProvider serviceProvider)
		{
			if (Client != null)
			{
				return Client;
			}

			ServiceProvider = serviceProvider;

			Client = new TelegramBotClient("626205769:AAFc8pYf1QMiF0zQQQKbAJOz038VsL6T1aQ");
			await Client.SetWebhookAsync($"{AppLink}/api/message/playzoneupdate");
			return Client;
		}
		
		public static async Task ProcessCallbackMessage(CallbackQuery callback)
		{
			var callBackMessage = callback.Data;
			var message = callback.Message;

			var parts = callBackMessage.Split("_");
			if (parts.Length == 2 && Enum.TryParse(typeof(RequestStatus), parts[0], out var status))
			{
				var requestId = Convert.ToInt32(parts[1]);
				using (var serviceScope =
					ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
				{
					var dbContext = serviceScope.ServiceProvider.GetService<PlayZoneDbContext>();
					var request = dbContext.UserRequests.First(u => u.Id == requestId);
					request.Status = (RequestStatus)status;
					dbContext.Update(request);
					dbContext.SaveChanges();
				}

				await Client.EditMessageTextAsync(message.Chat.Id, message.MessageId,
					message.Text + $"\r\n\r\nЗАЯВКА {status.GetDisplayName().ToUpper()}");
			}
		}

		public static async Task ProcessMessage(Message message)
		{
			try
			{
				var userId = message.Chat.Id;

				/*if (Enum.GetValues(typeof(PlayZone)).Cast<long>().Contains(userId))
				{
					return;
				}*/

				RequestsData.TryGetValue(userId, out var userData);

				if (userData == null || message.Text == "/start" || message.Text == "Забронировать ещё")
				{
					var contact = message.Chat;
					userData = new UserRequest
					{
						UserId = userId,
						ContactFirstName = contact.FirstName,
						ContactLastName = contact.LastName,
						CreateDate = DateTime.Now,
						Step = RequestStep.Zone,
						Status = RequestStatus.InProgress
					};

					RequestsData.Remove(userId);
					RequestsData.Add(userId, userData);
				}

				if (userData.Step == RequestStep.Zone)
				{
					if (string.IsNullOrEmpty(message.Text) || !PlayZones.TryGetValue(message.Text, out var value))
					{
						await Client.SendTextMessageAsync(message.Chat.Id,
							"Привет! Я Play Zone бот, я работаю без выходных на благо всех геймеров Харькова и помогу тебе забронировать любимую игру.");
						await Client.SendTextMessageAsync(message.Chat.Id, "Выбери игровую зону",
							replyMarkup: GetButtons(typeof(PlayZone)));
						return;
					}

					message.Text = "";
					userData.ZoneId = value;
					userData.Step++;
				}

				if (userData.Step == RequestStep.NumberOfPeople)
				{
					if (string.IsNullOrEmpty(message.Text) || !NumberOfPeoples.TryGetValue(message.Text, out var value))
					{
						await Client.SendTextMessageAsync(message.Chat.Id,
							"Сколько человек будет играть? (Если больше, уточним у тебя по телефону)",
							replyMarkup: GetButtons(typeof(NumberOfPeople)));
						return;
					}

					message.Text = "";
					userData.NumberOfPeople = value;
					userData.Step++;
				}

				if (userData.Step == RequestStep.GameConsole)
				{
					if (string.IsNullOrEmpty(message.Text) || !GameConsoles.TryGetValue(message.Text, out var value))
					{
						await Client.SendTextMessageAsync(message.Chat.Id,
							"Выберите свою приставку",
							replyMarkup: GetButtons(userData.ZoneId == PlayZone.Caravan
								? new List<GameConsole>
									{GameConsole.PlayStation, GameConsole.XboxOne, GameConsole.PsVr}
								: userData.ZoneId == PlayZone.Ukraine
									? new List<GameConsole> {GameConsole.PlayStation, GameConsole.XboxOne}
									: new List<GameConsole> {GameConsole.PlayStation}));
						return;
					}

					message.Text = "";
					userData.GameConsole = value;
					userData.Step++;
				}

				if (userData.Step == RequestStep.Game)
				{
					if (string.IsNullOrEmpty(message.Text) || !Games.TryGetValue(message.Text, out var value))
					{
						await Client.SendTextMessageAsync(message.Chat.Id,
							"В какую игру будешь играть?", replyMarkup: GetButtons(typeof(Game)));
						return;
					}

					if (value != Game.MyChoice)
					{
						userData.Game = message.Text;
						message.Text = "";
						userData.Step += 2;
					}
					else
					{
						message.Text = "";
						userData.Step++;
					}
				}

				if (userData.Step == RequestStep.GameInput)
				{
					if (string.IsNullOrEmpty(message.Text))
					{
						await Client.SendTextMessageAsync(message.Chat.Id, "Впишите название игры:",
							replyMarkup: new ForceReplyMarkup());
						return;
					}

					userData.Game = message.Text;
					message.Text = "";
					userData.Step++;
				}

				if (userData.Step == RequestStep.RequestDate)
				{
					if (string.IsNullOrEmpty(message.Text) || !RequestDates.TryGetValue(message.Text, out var value))
					{
						await Client.SendTextMessageAsync(message.Chat.Id,
							"Когда будешь играть?", replyMarkup: GetButtons(typeof(RequestDate)));
						return;
					}

					if (value != RequestDate.MyChoice)
					{
						userData.RequestDate = value == RequestDate.Today
							? DateTime.Now.Date
							: DateTime.Now.AddDays(1).Date;
						message.Text = "";
						userData.Step += 2;
					}
					else
					{
						message.Text = "";
						userData.Step++;
					}
				}

				if (userData.Step == RequestStep.RequestDateInput)
				{
					if (string.IsNullOrEmpty(message.Text))
					{
						await Client.SendTextMessageAsync(message.Chat.Id, "Впишите дату:",
							replyMarkup: new ForceReplyMarkup());
						return;
					}

					if (!DateTime.TryParseExact(message.Text, "dd.MM.yyyy", null,
						DateTimeStyles.None, out var requestDate))
					{
						await Client.SendTextMessageAsync(message.Chat.Id,
							"Какое-то странное число ты вводишь. Попробуй в формате \"28.01.2019\"");
						return;
					}

					if (requestDate < DateTime.Now.Date)
					{
						await Client.SendTextMessageAsync(message.Chat.Id, "Вы не можете поиграть задним числом)))");
						return;
					}

					userData.RequestDate = requestDate;
					message.Text = "";
					userData.Step++;
				}

				if (userData.Step == RequestStep.RequestTime)
				{
					if (string.IsNullOrEmpty(message.Text) || !RequestTimes.TryGetValue(message.Text, out var value))
					{
						await Client.SendTextMessageAsync(message.Chat.Id,
							"Выбери время (формат 19:00)", replyMarkup: GetButtons(typeof(RequestTime)));
						return;
					}

					if (value != RequestTime.MyChoice)
					{
						switch (value)
						{
							case RequestTime.Hours12:
								userData.RequestDate = userData.RequestDate.Value.AddHours(12);
								break;
							case RequestTime.Hours14:
								userData.RequestDate = userData.RequestDate.Value.AddHours(14);
								break;
							case RequestTime.Hours16:
								userData.RequestDate = userData.RequestDate.Value.AddHours(16);
								break;
							case RequestTime.Hours18:
								userData.RequestDate = userData.RequestDate.Value.AddHours(18);
								break;
							case RequestTime.Hours19:
								userData.RequestDate = userData.RequestDate.Value.AddHours(19);
								break;
						}

						message.Text = "";
						userData.Step += 2;
					}
					else
					{
						message.Text = "";
						userData.Step++;
					}
				}

				if (userData.Step == RequestStep.RequestTimeInput)
				{
					if (string.IsNullOrEmpty(message.Text))
					{
						await Client.SendTextMessageAsync(message.Chat.Id, "Впишите время:",
							replyMarkup: new ForceReplyMarkup());
						return;
					}

					if (!DateTime.TryParseExact(message.Text, "HH:mm", null,
						DateTimeStyles.None, out var requestDate))
					{
						await Client.SendTextMessageAsync(message.Chat.Id,
							"Что-от не очень похоже на удачное время. Попробуй в формате \"19:30\"");
						return;
					}

					if (userData.RequestDate.Value.Add(requestDate.TimeOfDay) < DateTime.Now)
					{
						await Client.SendTextMessageAsync(message.Chat.Id,
							"Вы не можете поиграть задним числом)))");
						return;
					}

					userData.RequestDate = userData.RequestDate.Value.Add(requestDate.TimeOfDay);
					message.Text = "";
					userData.Step++;
				}

				if (userData.Step == RequestStep.UserName)
				{
					if (string.IsNullOrEmpty(message.Text))
					{
						await Client.SendTextMessageAsync(message.Chat.Id, "Как тебя зовут?",
							replyMarkup: new ForceReplyMarkup());
						return;
					}

					userData.UserName = message.Text;
					message.Text = "";
					userData.Step++;
				}

				if (userData.Step == RequestStep.UserPhone)
				{
					if (string.IsNullOrEmpty(message.Text))
					{
						await Client.SendTextMessageAsync(message.Chat.Id, "Твой номер телефона:",
							replyMarkup: new ForceReplyMarkup());
						return;
					}

					userData.UserPhone = message.Text;
					message.Text = "";
					userData.Step++;
				}

				if (userData.Step == RequestStep.Finish)
				{
					await Client.SendTextMessageAsync(message.Chat.Id,
						"Круто! Ждем тебя в Play Zone!\r\nМы перезвоним для подтверждения бронирования.",
						replyMarkup: new ReplyKeyboardMarkup(new KeyboardButton("Забронировать ещё")));

					using (var serviceScope =
						ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
					{
						var dbContext = serviceScope.ServiceProvider.GetService<PlayZoneDbContext>();
						dbContext.UserRequests.Add(userData);
						dbContext.SaveChanges();
					}

					await Client.SendTextMessageAsync((int)userData.ZoneId,
						"Новая заявка:\r\n" +
						$"Имя: {userData.UserName}\r\n" +
						$"Телефон: {userData.UserPhone}\r\n" +
						$"На какое время: {userData.RequestDate.Value:dd-MM-yyyy HH:mm}\r\n" +
						$"Количество людей: {userData.NumberOfPeople.GetDisplayName()}\r\n" +
						$"Консоль: {userData.GameConsole.GetDisplayName()}\r\n" +
						$"Игра: {userData.Game}\r\n" +
						$"{AppLink}/home/EditRequest?id={userData.Id}",
						replyMarkup: new InlineKeyboardMarkup(new[]
						{
							InlineKeyboardButton.WithCallbackData("Подтвердить", $"Approved_{userData.Id}"),
							InlineKeyboardButton.WithCallbackData("Отклонить", $"Rejected_{userData.Id}")
						}));

					RequestsData.Remove(userId);
				}
			}
			catch (Exception ex)
			{
			}
		}

		private static ReplyKeyboardMarkup GetButtons(Type enumType)
		{
			return GetButtons(Enum.GetValues(enumType).Cast<object>().ToList());
		}
		
		private static ReplyKeyboardMarkup GetButtons<T>(List<T> data)
		{
			var structured = new List<KeyboardButton[]>();
			var step = data.Count > 3 ? 2 : 1;
			for (var i = 0; i < data.Count; i = i + step)
			{
				structured.Add(data.Skip(i).Take(step)
					.Select(c => new KeyboardButton(c.GetDisplayName())).ToArray());
			}

			return new ReplyKeyboardMarkup
			{
				Keyboard = structured.ToArray(),
				ResizeKeyboard = true
			};
		}
	}
}