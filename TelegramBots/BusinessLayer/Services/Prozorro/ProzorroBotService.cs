using BusinessLayer.Helpers;
using DataLayer.Context;
using DataLayer.Models.DTO;
using DataLayer.Models.Prozorro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DataLayer.Models.Prozorro.User;

namespace BusinessLayer.Services.Prozorro
{
	public class ProzorroBotService
	{
		private readonly ProzorroDbContext _repository;
		public static string[][] RegisterKeywboard;
		public static string[][] MainKeyboard;
		public static TelegramBotClient Client;
		public static string ApiUrl = "https://public.api.openprocurement.org/api/0/tenders/{0}";

		static ProzorroBotService()
		{
			RegisterKeywboard = new[]
			{
				new[] {PhraseHelper.Register}
			};
			MainKeyboard = new[]
			{
				new[] {PhraseHelper.FindTender, PhraseHelper.MySubscriptions}
			};
		}

		public ProzorroBotService(ProzorroDbContext repository)
		{
			_repository = repository;
		}

		public static void Init()
		{
			if (Client == null)
			{
				Client = new TelegramBotClient(ConfigData.TelegramProzorroKey);
			}
		}

		public static async Task SetWebHook()
		{
			await Client.DeleteWebhookAsync();
			await Client.SetWebhookAsync($"{ConfigData.AppLink}/api/message/prozorroupdate");
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
				if (callback.Data.Contains(PhraseHelper.UnsubscribeCode))
				{
					var unsubscribeId = Convert.ToInt32(callback.Data.Replace(PhraseHelper.UnsubscribeCode, ""));
					var subscriptionData = _repository.Subscriptions.FirstOrDefault(s => s.Id == unsubscribeId);

					if (subscriptionData == null)
					{
						return;
					}

					_repository.Remove(subscriptionData);
					_repository.SaveChanges();

					/*await SendTextMessage(new AnswerMessageBase(message.Chat.Id,
						PhraseHelper.SuccessfullyUnsubscribeFromTender, MainKeyboard));*/
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
				if (chatId < 0)
				{
					await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.BotDontWorkWithGroups));
				}

				errorUserId = chatId;
				var userFirstName = message.Chat.FirstName;
				var userLastName = message.Chat.LastName;
				var userName = message.Chat.Username;
				var user = GetUserInfo(chatId, userFirstName, userLastName, userName);
				errorUserId = user.Id;

				if (message.Type == MessageType.Contact)
				{
					if (!user.IsRegistered)
					{
						user.Phone = message.Contact.PhoneNumber;
						user.RegistrationDate = DateTime.Now;
						_repository.SaveChanges();

						await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.RegistrationSuccess,
							MainKeyboard));
					}
					else
					{
						await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.YouAlreadyRegistered,
							MainKeyboard));
					}

					return;
				}

				if (messageText.StartsWith("UA-"))
				{
					var tender = _repository.Tenders.FirstOrDefault(t => t.Code == messageText);
					if (tender == null)
					{
						var tenderUrl = $"https://prozorro.gov.ua/tender/{messageText}";
						var tenderToken = "";
						var request = (HttpWebRequest)WebRequest.Create(tenderUrl);
						var htmlResponse = (HttpWebResponse)request.GetResponse();

						if (htmlResponse.StatusCode == HttpStatusCode.OK)
						{
							using (var receiveStream = htmlResponse.GetResponseStream())
							{
								if (receiveStream != null)
								{
									StreamReader readStream;
									if (string.IsNullOrWhiteSpace(htmlResponse.CharacterSet))
									{
										readStream = new StreamReader(receiveStream);
									}
									else
									{
										readStream = new StreamReader(receiveStream,
											Encoding.GetEncoding(htmlResponse.CharacterSet));
									}

									var htmlData = readStream.ReadToEnd();
									var headInfoPart = htmlData.Substring(htmlData.IndexOf("tender--head--inf",
										StringComparison.Ordinal));
									var tokenPart = headInfoPart.Substring(
										headInfoPart.IndexOf("</span>", StringComparison.Ordinal) + 7);
									tenderToken = tokenPart.Substring(0,
										tokenPart.IndexOf("</div>", StringComparison.Ordinal)).Trim();
								}
							}

							htmlResponse.Close();
						}

						if (string.IsNullOrEmpty(tenderToken))
						{
							await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.InvalidTender,
								MainKeyboard));
							return;
						}

						tender = new Tender
						{
							Code = messageText,
							Token = tenderToken
						};

						_repository.Add(tender);
						_repository.SaveChanges();
					}
					
					var tenderApiData = GetTenderData(string.Format(ApiUrl, tender.Token));

					UpdateTenderData(tender, tenderApiData);
					_repository.SaveChanges();
					await Monitoring(tender.Id);

					var tenderInfo = new StringBuilder();
					tenderInfo.AppendLine($"Заголовок: {tenderApiData.Title}");
					tenderInfo.AppendLine($"Стоимость: {tenderApiData.Value.Amount} {tenderApiData.Value.Currency}");
					tenderInfo.AppendLine("Заказчик:");
					tenderInfo.AppendLine(tenderApiData.ProcuringEntity.Name);
					tenderInfo.AppendLine(tenderApiData.ProcuringEntity.ContactPoint.Name);
					tenderInfo.AppendLine(tenderApiData.ProcuringEntity.ContactPoint.Telephone);
					tenderInfo.AppendLine(tenderApiData.ProcuringEntity.ContactPoint.Email);

					var subscribeKeyboard = MainKeyboard.ToList();
					if (!_repository.Subscriptions.Any(s => s.UserId == user.Id && s.TenderId == tender.Id))
					{
						subscribeKeyboard.Insert(0, new[] {PhraseHelper.SubscribeOnTender + tenderApiData.TenderId});
					}

					await SendTextMessage(new AnswerMessageBase(chatId, tenderInfo.ToString(),
						subscribeKeyboard.ToArray()));
					return;
				}

				if (messageText.StartsWith(PhraseHelper.SubscribeOnTender))
				{
					var tenderCode = messageText.Replace(PhraseHelper.SubscribeOnTender, "");
					var tenderId = _repository.Tenders.First(t => t.Code == tenderCode).Id;
					_repository.Add(new Subscription
					{
						UserId = user.Id,
						TenderId = tenderId
					});
					_repository.SaveChanges();

					await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.SuccessfullySubscribeOnTender,
						MainKeyboard));
					return;
				}

				switch (messageText)
				{
					case PhraseHelper.Start:
					{
						await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.ProzorroAboutBotText,
							user.IsRegistered ? MainKeyboard : RegisterKeywboard));
						return;
					}
					case PhraseHelper.FindTender:
					{
						await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.HowToFindTender,
							MainKeyboard));
						return;
					}
					case PhraseHelper.MySubscriptions:
					{
						foreach (var subscription in _repository.Subscriptions
							.Where(s => s.UserId == user.Id).Select(s => new {s.Id, s.Tender.Title, s.Tender.Code}))
						{
							await SendTextMessage(
								new AnswerMessageBase(chatId, $"{subscription.Title}\r\n{subscription.Code}")
								{
									InlineKeyboard = new Dictionary<string, string>
									{
										{PhraseHelper.Unsubscribe, $"{PhraseHelper.UnsubscribeCode}{subscription.Id}"}
									}
								});
						}

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

		public async Task Monitoring(int? tenderId = null)
		{
			var subscriptions = _repository.Subscriptions.Where(s => !tenderId.HasValue || s.TenderId == tenderId)
				.Select(s => new {s.TenderId, s.User.ChatId}).GroupBy(s => s.TenderId).ToList();
			var tenders = _repository.Tenders.Where(t => tenderId.HasValue ? t.Id == tenderId : t.Subscribers.Any())
				.ToList();

			foreach (var subscription in subscriptions)
			{
				var tenderCurrentData = tenders.First(t => t.Id == subscription.Key);
				var tenderApiData = GetTenderData(string.Format(ApiUrl, tenderCurrentData.Token));

				var needToSendUpdateMessage = false;
				var updateText = new StringBuilder();
				updateText.AppendLine($"Обновление тендера: {tenderApiData.Title} ({tenderApiData.TenderId})");
				if (tenderCurrentData.Status != tenderApiData.Status)
				{
					updateText.AppendLine($"Статус изменен с {tenderCurrentData.Status} на {tenderApiData.Status}");
					needToSendUpdateMessage = true;
				}
				if (tenderCurrentData.Amount != tenderApiData.Value.Amount)
				{
					updateText.AppendLine($"Цена изменена с {tenderCurrentData.Amount} на {tenderApiData.Value.Amount}");
					needToSendUpdateMessage = true;
				}

				if (needToSendUpdateMessage)
				{
					foreach (var user in subscription)
					{
						await SendTextMessage(new AnswerMessageBase(user.ChatId, updateText.ToString()));
					}
				}

				UpdateTenderData(tenderCurrentData, tenderApiData);
			}

			_repository.SaveChanges();
		}

		private User GetUserInfo(long chatId, string userFirstName, string userLastName, string userName)
		{
			var user = _repository.Users.FirstOrDefault(u => u.ChatId == chatId);
			if (user == null)
			{
				user = new User
				{
					ChatId = chatId,
					FirstName = userFirstName,
					LastName = userLastName,
					UserName = userName,
					JoinDate = DateTime.Now
				};
				_repository.Add(user);
				_repository.SaveChanges();
			}
			else
			{
				if (string.IsNullOrEmpty(user.UserName) && !string.IsNullOrEmpty(userName))
				{
					user.UserName = userName;
					_repository.SaveChanges();
				}
			}

			return user;
		}

		private static TenderDTO GetTenderData(string tenderTokenUrl)
		{
			var webRequest = (HttpWebRequest)WebRequest.Create(tenderTokenUrl);
			webRequest.Method = "Get";
			var response = (HttpWebResponse)webRequest.GetResponse();
			var responseData = "";

			using (var stream = response.GetResponseStream())
			{
				if (stream != null)
				{
					using (var reader = new StreamReader(stream, Encoding.UTF8))
					{
						responseData = reader.ReadToEnd();
					}
				}
			}

			response.Close();
			return JsonConvert.DeserializeObject<Dictionary<string, TenderDTO>>(responseData)
				.First().Value;
		}

		private static void UpdateTenderData(Tender currentTenderData, TenderDTO newTenderData)
		{
			currentTenderData.Token = newTenderData.Id;
			currentTenderData.Code = newTenderData.TenderId;
			currentTenderData.Title = newTenderData.Title;
			currentTenderData.Status = newTenderData.Status;
			currentTenderData.Amount = newTenderData.Value.Amount;
			currentTenderData.Currency = newTenderData.Value.Currency;
		}
	}
}