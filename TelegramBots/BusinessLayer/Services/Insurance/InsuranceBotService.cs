using System;
using BusinessLayer.Helpers;
using DataLayer.Models.DTO;
using DataLayer.Models.Insurance;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataLayer.Models.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace BusinessLayer.Services.Insurance
{
	public class InsuranceBotService
	{
		public static TelegramBotClient Client;
		public static Dictionary<long, UserRequest> RequestsData = new Dictionary<long, UserRequest>();
		public static string[][] MainKeyboard;
		public static string[][] MainStepsKeyboard;
		public string WebRootPath;

		static InsuranceBotService()
		{
			MainKeyboard = new[]
			{
				new[] {PhraseHelper.InsuranceStartFromBegining}
			};

			MainStepsKeyboard = new[]
			{
				PhraseHelper.InsuranceMainSteps.Select(d => d.Key.ToString()).ToArray()
			};
		}

		public static void Init()
		{
			if (Client == null)
			{
				Client = new TelegramBotClient(ConfigData.TelegramInsuranceKey);
			}
		}

		public static async Task SetWebHook()
		{
			await Client.DeleteWebhookAsync();
			await Client.SetWebhookAsync($"{ConfigData.AppLink}/api/message/insuranceupdate");
		}

		public async Task<Message> SendTextMessage(AnswerMessageBase message)
		{
			return await Client.SendTextMessage(message);
		}

		public async Task ProcessCallbackMessage(CallbackQuery callback)
		{
		}

		public async Task ProcessMessage(Message message)
		{
			await ProcessMessageBase(message, message.Text);
		}

		public async Task ProcessMessageBase(Message message, string messageText)
		{
			var chatId = message.Chat.Id;
			if (chatId < 0)
			{
				await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.BotDontWorkWithGroups));
				return;
			}

			if (!RequestsData.TryGetValue(chatId, out var userInfo))
			{
				userInfo = new UserRequest
				{
					UserId = chatId,
					Step = InsuranceStep.Start
				};
				RequestsData.Add(chatId, userInfo);
			}

			try
			{
				switch (messageText)
				{
					case PhraseHelper.InsuranceOperation1:
					{
						userInfo.Step = InsuranceStep.Operation1Start;
						break;
					}
					case PhraseHelper.InsuranceOperation2:
					{
						userInfo.Step = InsuranceStep.Operation2Start;
						break;
					}
					case PhraseHelper.InsuranceOperation3:
					{
						userInfo.Step = InsuranceStep.Operation3Preview;
						break;
					}
					case PhraseHelper.InsuranceSendDocumentsToMail:
					{
						userInfo.Step = InsuranceStep.Operation3Start;
						messageText = PhraseHelper.InsuranceOperation3;
						break;
					}
					case PhraseHelper.InsuranceOperation4:
					{
						userInfo.Step = InsuranceStep.Operation4Start;
						break;
					}
					case PhraseHelper.InsuranceOperation5:
					{
						await SendTextMessage(
							new AnswerMessageBase(chatId, PhraseHelper.InsuranceContacts, MainKeyboard)
								{IsHtml = true});
						return;
					}
					default:
					{
						if (userInfo.Step == InsuranceStep.Start ||
						    messageText == PhraseHelper.InsuranceStartFromBegining)
						{
							ClearUserInfo(userInfo);
							SendStartMessage(chatId);
							return;
						}

						break;
					}
				}

				var isFirstStepInOperation = userInfo.Step.ToString().EndsWith("Start");

				if (userInfo.Step == InsuranceStep.Operation2Step3)
				{
					if (message.Type == MessageType.Photo)
					{
						var photoId = message.Photo.Last().FileId;
						var photoInfo = await Client.GetFileAsync(photoId);
						var webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(
							$"https://api.telegram.org/file/bot{ConfigData.TelegramInsuranceKey}/{photoInfo.FilePath}");
						webRequest.AllowWriteStreamBuffering = true;
						var webResponse = webRequest.GetResponse();
						userInfo.Photo = webResponse.GetResponseStream();
						userInfo.PhotoName = photoInfo.FilePath.Split('/').Last();
					}
					else
					{
						await SendTextMessage(new AnswerMessageBase(chatId,
							PhraseHelper.InsuranceStepsText[userInfo.Step], MainKeyboard));
						return;
					}
				}
				else if (userInfo.Step == InsuranceStep.Operation3Preview)
				{
					await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.InsuranceOperation3DocumentsText,
						MainKeyboard));

					await using var sourceStream =
						System.IO.File.Open(Path.Combine(WebRootPath, "documents", "documents.docx"), FileMode.Open);
					await Client.SendDocumentAsync(chatId,
						new InputOnlineFile(sourceStream, "documents.docx"),
						replyMarkup: KeyboardHelper.GetKeyboardTelegram(MainKeyboard));
					Thread.Sleep(3000);
					await SendTextMessage(new AnswerMessageBase(chatId,
						PhraseHelper.InsuranceOperation3AddressForDocuments,
						new[]
						{
							new[] {PhraseHelper.InsuranceSendDocumentsToMail},
							new[] {PhraseHelper.InsuranceStartFromBegining}
						}));

					return;
				}
				else if (userInfo.Step == InsuranceStep.Operation3Step1)
				{
					try
					{
						var unused = new MailAddress(messageText);
					}
					catch
					{
						await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.InsuranceWrondFormat,
							MainKeyboard));
						await SendTextMessage(new AnswerMessageBase(chatId,
							PhraseHelper.InsuranceStepsText[userInfo.Step], MainKeyboard));
						return;
					}
				}

				userInfo.Text += (isFirstStepInOperation
					                 ? ""
					                 : $"<b>{PhraseHelper.InsuranceStepsText[userInfo.Step]}:</b> ") +
				                 $"{(isFirstStepInOperation ? PhraseHelper.InsuranceMainSteps[messageText] : messageText)}<br/>";

				userInfo.Step++;
				var botText = PhraseHelper.InsuranceStepsText[userInfo.Step];
				await SendTextMessage(new AnswerMessageBase(chatId, botText, MainKeyboard));

				if (userInfo.Step.ToString().EndsWith("End"))
				{
					SmtpManager.CreateAndSendEmail(userInfo.Text, "Message from telegram bot", "aboikovskyi@gmail.com",
						userInfo.Photo != null ? new Attachment(userInfo.Photo, userInfo.PhotoName) : null);
					ClearUserInfo(userInfo);
					Thread.Sleep(3000);
					SendStartMessage(chatId);
				}
			}
			catch (Exception ex)
			{
				await using (var fs = System.IO.File.Open(Path.Combine(WebRootPath, "log.txt"), FileMode.OpenOrCreate))
				{
					var info = new UTF8Encoding(true).GetBytes(
						$"\r\n{DateTime.Now}\r\n{ex.Message}\r\n{ex.StackTrace}\r\n");
					fs.Write(info, 0, info.Length);
				}

				ClearUserInfo(userInfo);
				SendStartMessage(chatId);
			}
		}

		private static void ClearUserInfo(UserRequest userInfo)
		{
			userInfo.Step = InsuranceStep.Start;
			userInfo.Text = null;
			userInfo.Photo = null;
			userInfo.PhotoName = null;
		}

		private async void SendStartMessage(long chatId)
		{
			await SendTextMessage(new AnswerMessageBase(chatId,
				$"{PhraseHelper.InsuranceStartText}\r\n\r\n{string.Join("\r\n", PhraseHelper.InsuranceMainSteps.Select(s => s.Value))}",
				MainStepsKeyboard));
		}
	}
}