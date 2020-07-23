﻿using System;
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
using Telegram.Bot.Types.ReplyMarkups;

namespace BusinessLayer.Services.Insurance
{
	public class InsuranceBotService
	{
		public static TelegramBotClient Client;
		public static Dictionary<long, UserRequest> RequestsData = new Dictionary<long, UserRequest>();
		public static Dictionary<string, string> MainKeyboard;
		public static Dictionary<string, string> MainStepsKeyboard;
		public static Dictionary<InsuranceStep, string> InsuranceStepData;
		public static string WebRootPath;
		public const string Operation1Code = "operation1";
		public const string Operation2Code = "operation2";
		public const string Operation3Code = "operation3";
		public const string Operation4Code = "operation4";
		public const string Operation5Code = "operation5";
		public const string SendDocumentsToEmailCode = "sendDocumentsToEmail";

		public InsuranceBotService()
		{
			var phrases = InsurancePhraseHelper.GetPhrases();
			MainKeyboard = new Dictionary<string, string> {{phrases.StartFromBegining, "/start"}};
			MainStepsKeyboard = new Dictionary<string, string>
			{
				{phrases.Operation1, Operation1Code},
				{phrases.Operation2, Operation2Code},
				{phrases.Operation3, Operation3Code},
				{phrases.Operation4, Operation4Code},
				{phrases.Operation5, Operation5Code}
			};

			InsuranceStepData = new Dictionary<InsuranceStep, string>
			{
				{InsuranceStep.Operation1Step1, phrases.Operation1Step1},
				{InsuranceStep.Operation1Step2, phrases.Operation1Step2},
				{InsuranceStep.Operation1Step3, phrases.Operation1Step3},
				{InsuranceStep.Operation1Step4, phrases.Operation1Step4},
				{InsuranceStep.Operation1Step5, phrases.Operation1Step5},
				{InsuranceStep.Operation1End, phrases.Operation1End},
				{InsuranceStep.Operation2Step1, phrases.Operation2Step1},
				{InsuranceStep.Operation2Step2, phrases.Operation2Step2},
				{InsuranceStep.Operation2Step3, phrases.Operation2Step3},
				{InsuranceStep.Operation2Step4, phrases.Operation2Step4},
				{InsuranceStep.Operation2End, phrases.Operation2End},
				{InsuranceStep.Operation3Step1, phrases.Operation3Step1},
				{InsuranceStep.Operation3End, phrases.Operation3End},
				{InsuranceStep.Operation4Step1, phrases.Operation4Step1},
				{InsuranceStep.Operation4Step2, phrases.Operation4Step2},
				{InsuranceStep.Operation4End, phrases.Operation4End},
			};
		}

		public static void Init()
		{
			Client ??= new TelegramBotClient(ConfigData.TelegramInsuranceKey);
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
			var message = callback.Message;
			await ProcessMessageBase(message, callback.Data);
		}

		public async Task ProcessMessage(Message message)
		{
			await ProcessMessageBase(message, message.Text);
		}

		public async Task ProcessMessageBase(Message message, string messageText)
		{
			var phrases = InsurancePhraseHelper.GetPhrases();
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
					case Operation1Code:
					{
						userInfo.Step = InsuranceStep.Operation1Start;
						break;
					}
					case Operation2Code:
					{
						userInfo.Step = InsuranceStep.Operation2Start;
						break;
					}
					case Operation3Code:
					{
						userInfo.Step = InsuranceStep.Operation3Preview;
						break;
					}
					case SendDocumentsToEmailCode:
					{
						userInfo.Step = InsuranceStep.Operation3Start;
						messageText = phrases.Operation3;
						break;
					}
					case Operation4Code:
					{
						userInfo.Step = InsuranceStep.Operation4Start;
						break;
					}
					case Operation5Code:
					{
						await SendTextMessage(
							new AnswerMessageBase(chatId, phrases.Contacts, MainKeyboard)
								{IsHtml = true});
						return;
					}
					default:
					{
						if (userInfo.Step == InsuranceStep.Start || messageText == PhraseHelper.Start)
						{
							ClearUserInfo(userInfo);
							SendStartMessage(chatId, phrases);
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
					userInfo.Text +=
						$"<b>{MainStepsKeyboard.FirstOrDefault(s => s.Value == Operation3Code).Key}:</b><br/>";

					await SendTextMessage(new AnswerMessageBase(chatId, phrases.Operation3DocumentsText, MainKeyboard));

					await using var sourceStream =
						System.IO.File.Open(Path.Combine(WebRootPath, "documents", "Заява на виплату.doc"),
							FileMode.Open);
					await Client.SendDocumentAsync(chatId, new InputOnlineFile(sourceStream, "Заява на виплату.doc"));
					Thread.Sleep(3000);
					await SendTextMessage(new AnswerMessageBase(chatId,
						phrases.Operation3AddressForDocuments,
						new Dictionary<string, string>
						{
							{phrases.SendDocumentsToMail, SendDocumentsToEmailCode},
							{phrases.StartFromBegining, PhraseHelper.Start}
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
				                 $"{(isFirstStepInOperation ? MainStepsKeyboard.FirstOrDefault(s => s.Value == messageText).Key : messageText)}<br/>";

				userInfo.Step++;
				var botText = PhraseHelper.InsuranceStepsText[userInfo.Step];
				await SendTextMessage(new AnswerMessageBase(chatId, botText, MainKeyboard));

				if (userInfo.Step.ToString().EndsWith("End"))
				{
					SmtpManager.CreateAndSendEmail(userInfo.Text, "Message from telegram bot",
						ConfigData.EmailTo, ConfigData.EmailCopy,
						userInfo.Photo != null ? new Attachment(userInfo.Photo, userInfo.PhotoName) : null);
					ClearUserInfo(userInfo);
					Thread.Sleep(3000);
					SendStartMessage(chatId, phrases);
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
				SendStartMessage(chatId, phrases);
			}
		}

		private static void ClearUserInfo(UserRequest userInfo)
		{
			userInfo.Step = InsuranceStep.Start;
			userInfo.Text = null;
			userInfo.Photo = null;
			userInfo.PhotoName = null;
		}

		private async void SendStartMessage(long chatId, InsurancePhrases phrases)
		{
			await SendTextMessage(new AnswerMessageBase(chatId, phrases.StartText, MainStepsKeyboard)
				{IsHtml = true});
		}
	}
}