using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Helpers;
using DataLayer.Context;
using DataLayer.Helpers;
using DataLayer.Models;
using DataLayer.Models.Enums;
using TelegramBots.Models;
using Game = DataLayer.Models.Enums.Game;

namespace BusinessLayer.Services.PlayZone
{
	public class PlayZoneBotServiceBase
	{
		private readonly PlayZoneDbContext _context;
		public static Dictionary<string, UserRequest> RequestsData = new Dictionary<string, UserRequest>();
		public static Dictionary<string, DataLayer.Models.Enums.PlayZone> PlayZones;
		public static Dictionary<string, NumberOfPeople> NumberOfPeoples;
		public static Dictionary<string, GameConsole> GameConsoles;
		public static Dictionary<string, Game> Games;
		public static Dictionary<string, RequestDate> RequestDates;
		public static Dictionary<string, RequestTime> RequestTimes;

		static PlayZoneBotServiceBase()
		{
			PlayZones = Enum.GetValues(typeof(DataLayer.Models.Enums.PlayZone)).Cast<DataLayer.Models.Enums.PlayZone>()
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

		public PlayZoneBotServiceBase(PlayZoneDbContext context)
		{
			_context = context;
		}

		public object ProcessCallbackMessageBase(string callBackMessage, long chatIt, int messageId,
			string messageText)
		{
			var parts = callBackMessage.Split("_");
			if (parts.Length == 2 && Enum.TryParse(typeof(RequestStatus), parts[0], out var status))
			{
				var requestId = Convert.ToInt32(parts[1]);

				var request = _context.UserRequests.First(u => u.Id == requestId);
				request.Status = (RequestStatus)status;
				_context.Update(request);
				_context.SaveChanges();

				return status;
			}

			return null;
		}

		public virtual Task SendTextMessage(AnswerMessageBase message)
		{
			return Task.FromResult(default(object));
		}

		public async Task ProcessMessageBase(string userId, string userFirstName, string userLastName,
			string messageText)
		{
			try
			{
				RequestsData.TryGetValue(userId, out var userData);

				if (userData == null || messageText == "/start" || messageText == "Забронировать ещё")
				{
					userData = new UserRequest
					{
						UserId = userId,
						ContactFirstName = userFirstName,
						ContactLastName = userLastName,
						CreateDate = DateTime.Now,
						Step = RequestStep.Zone,
						Status = RequestStatus.InProgress
					};

					RequestsData.Remove(userId);
					RequestsData.Add(userId, userData);
				}

				if (userData.Step == RequestStep.Zone)
				{
					if (string.IsNullOrEmpty(messageText) || !PlayZones.TryGetValue(messageText, out var value))
					{
						await SendTextMessage(new AnswerMessageBase(userId,
							"Привет! Я Play Zone бот, я работаю без выходных на благо всех геймеров Харькова и помогу тебе забронировать любимую игру."));
						await SendTextMessage(new AnswerMessageBase(userId, "Выбери игровую зону", typeof(DataLayer.Models.Enums.PlayZone)));
						return;
					}

					messageText = "";
					userData.ZoneId = value;
					userData.Step++;
				}

				if (userData.Step == RequestStep.NumberOfPeople)
				{
					if (string.IsNullOrEmpty(messageText) || !NumberOfPeoples.TryGetValue(messageText, out var value))
					{
						await SendTextMessage(new AnswerMessageBase(userId,
							"Сколько человек будет играть? (Если больше, уточним у тебя по телефону)",
							typeof(NumberOfPeople)));
						return;
					}

					messageText = "";
					userData.NumberOfPeople = value;
					userData.Step++;
				}

				if (userData.Step == RequestStep.GameConsole)
				{
					if (string.IsNullOrEmpty(messageText) || !GameConsoles.TryGetValue(messageText, out var value))
					{
						await SendTextMessage(new AnswerMessageBase(userId,
							"Выберите свою приставку", userData.ZoneId == DataLayer.Models.Enums.PlayZone.Caravan
								? new List<object>
									{GameConsole.PlayStation, GameConsole.XboxOne, GameConsole.PsVr}
								: userData.ZoneId == DataLayer.Models.Enums.PlayZone.Ukraine
									? new List<object> {GameConsole.PlayStation, GameConsole.XboxOne}
									: new List<object> {GameConsole.PlayStation}));
						return;
					}

					messageText = "";
					userData.GameConsole = value;
					userData.Step++;
				}

				if (userData.Step == RequestStep.Game)
				{
					if (string.IsNullOrEmpty(messageText) || !Games.TryGetValue(messageText, out var value))
					{
						await SendTextMessage(
							new AnswerMessageBase(userId, "В какую игру будешь играть?", typeof(Game)));
						return;
					}

					if (value != Game.MyChoice)
					{
						userData.Game = messageText;
						messageText = "";
						userData.Step += 2;
					}
					else
					{
						messageText = "";
						userData.Step++;
					}
				}

				if (userData.Step == RequestStep.GameInput)
				{
					if (string.IsNullOrEmpty(messageText))
					{
						await SendTextMessage(
							new AnswerMessageBase(userId, "Впишите название игры:") {IsForceReplyMarkup = true});
						return;
					}

					userData.Game = messageText;
					messageText = "";
					userData.Step++;
				}

				if (userData.Step == RequestStep.RequestDate)
				{
					if (string.IsNullOrEmpty(messageText) || !RequestDates.TryGetValue(messageText, out var value))
					{
						await SendTextMessage(
							new AnswerMessageBase(userId, "Когда будешь играть?", typeof(RequestDate)));
						return;
					}

					if (value != RequestDate.MyChoice)
					{
						userData.RequestDate = value == RequestDate.Today
							? DateTime.Now.Date
							: DateTime.Now.AddDays(1).Date;
						messageText = "";
						userData.Step += 2;
					}
					else
					{
						messageText = "";
						userData.Step++;
					}
				}

				if (userData.Step == RequestStep.RequestDateInput)
				{
					if (string.IsNullOrEmpty(messageText))
					{
						await SendTextMessage(
							new AnswerMessageBase(userId, "Впишите дату:") {IsForceReplyMarkup = true});
						return;
					}

					if (!DateTime.TryParseExact(messageText, "dd.MM.yyyy", null,
						DateTimeStyles.None, out var requestDate))
					{
						await SendTextMessage(new AnswerMessageBase(userId,
							"Какое-то странное число ты вводишь. Попробуй в формате \"28.01.2019\""));
						return;
					}

					if (requestDate < DateTime.Now.Date)
					{
						await SendTextMessage(new AnswerMessageBase(userId, "Вы не можете поиграть задним числом)))"));
						return;
					}

					userData.RequestDate = requestDate;
					messageText = "";
					userData.Step++;
				}

				if (userData.Step == RequestStep.RequestTime)
				{
					if (string.IsNullOrEmpty(messageText) || !RequestTimes.TryGetValue(messageText, out var value))
					{
						await SendTextMessage(new AnswerMessageBase(userId, "Выбери время (формат 19:00)",
							typeof(RequestTime)));
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

						messageText = "";
						userData.Step += 2;
					}
					else
					{
						messageText = "";
						userData.Step++;
					}
				}

				if (userData.Step == RequestStep.RequestTimeInput)
				{
					if (string.IsNullOrEmpty(messageText))
					{
						await SendTextMessage(
							new AnswerMessageBase(userId, "Впишите время:") {IsForceReplyMarkup = true});
						return;
					}

					if (!DateTime.TryParseExact(messageText, "HH:mm", null,
						DateTimeStyles.None, out var requestDate))
					{
						await SendTextMessage(new AnswerMessageBase(userId,
							"Что-от не очень похоже на удачное время. Попробуй в формате \"19:30\""));
						return;
					}

					if (userData.RequestDate.Value.Add(requestDate.TimeOfDay) < DateTime.Now)
					{
						await SendTextMessage(new AnswerMessageBase(userId, "Вы не можете поиграть задним числом)))"));
						return;
					}

					userData.RequestDate = userData.RequestDate.Value.Add(requestDate.TimeOfDay);
					messageText = "";
					userData.Step++;
				}

				if (userData.Step == RequestStep.UserName)
				{
					if (string.IsNullOrEmpty(messageText))
					{
						await SendTextMessage(
							new AnswerMessageBase(userId, "Как тебя зовут?") {IsForceReplyMarkup = true});
						return;
					}

					userData.UserName = messageText;
					messageText = "";
					userData.Step++;
				}

				if (userData.Step == RequestStep.UserPhone)
				{
					if (string.IsNullOrEmpty(messageText))
					{
						await SendTextMessage(
							new AnswerMessageBase(userId, "Твой номер телефона:") {IsForceReplyMarkup = true});
						return;
					}

					userData.UserPhone = messageText;
					messageText = "";
					userData.Step++;
				}

				if (userData.Step == RequestStep.Finish)
				{
					await SendTextMessage(new AnswerMessageBase(userId,
						"Круто! Ждем тебя в Play Zone!\r\nМы перезвоним для подтверждения бронирования.",
						new List<object> {"Забронировать ещё"}));

					_context.UserRequests.Add(userData);
					_context.SaveChanges();

					await PlayZoneBotServiceTelegram.SendTextMessageStatic(new AnswerMessageBase(((int)userData.ZoneId).ToString(),
						"Новая заявка:\r\n" +
						$"Имя: {userData.UserName}\r\n" +
						$"Телефон: {userData.UserPhone}\r\n" +
						$"На какое время: {userData.RequestDate.Value:dd-MM-yyyy HH:mm}\r\n" +
						$"Количество людей: {userData.NumberOfPeople.GetDisplayName()}\r\n" +
						$"Консоль: {userData.GameConsole.GetDisplayName()}\r\n" +
						$"Игра: {userData.Game}\r\n" +
						$"{Links.AppLink}/home/EditRequest?id={userData.Id}")
					{
						InlineKeyboard = new Dictionary<string, string>
						{
							{"Подтвердить", $"Approved_{userData.Id}"},
							{"Отклонить", $"Rejected_{userData.Id}"}
						}
					});

					RequestsData.Remove(userId);
				}
			}
			catch (Exception ex)
			{
			}
		}
	}
}