using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Helpers;
using TelegramBots.Models;
using Viber.Bot;
using KeyboardButton = Viber.Bot.KeyboardButton;

namespace TelegramBots.Services
{
	public class PlayZoneBotServiceViber : PlayZoneBotServiceBase
	{
		public static ViberBotClient Client;

		public new const string AppLink = "https://playzone.ua";
		//public new const string AppLink = "https://84b65c6d.ngrok.io";
		public static string ViberAdminId = "+EevijqQxF1RlnZCJvQsyQ==";

		public static void Init(IServiceProvider serviceProvider)
		{
			if (Client == null)
			{
				ServiceProvider = serviceProvider;
				Client = new ViberBotClient("494050bd2f67d427-b0167b87f79c3f8e-323999acbc7b6097");
			}
		}

		public static async Task SetWebHook()
		{
			await Client.SetWebhookAsync("");
			await Client.SetWebhookAsync($"{AppLink}/api/message/playzoneviberupdate");
		}

		public override async Task SendTextMessage(AnswerMessageBase message)
		{
			if (message.KeyboardList != null)
			{
				await Client.SendKeyboardMessageAsync(new KeyboardMessage
				{
					Receiver = message.UserId,
					Sender = new UserBase
					{
						Name = "PlayZoneBot"
					},
					Text = message.Text,
					Keyboard = GetButtons(message.KeyboardList)
				});
			}
			else if (message.InlineKeyboard != null)
			{
				/*var inlineKeyboard = new InlineKeyboardMarkup(message.InlineKeyboard
					.Select(d => InlineKeyboardButton.WithCallbackData(d.Key, d.Value)).ToArray());
				await Client.SendTextMessageAsync(message.UserId, message.Text,
					replyMarkup: inlineKeyboard);*/
			}
			else if (message.IsForceReplyMarkup)
			{
				await Client.SendTextMessageAsync(new TextMessage
				{
					Receiver = message.UserId,
					Sender = new UserBase
					{
						Name = "PlayZoneBot"
					},
					Text = message.Text
				});
			}
			else
			{
				await Client.SendTextMessageAsync(new TextMessage
				{
					Receiver = message.UserId,
					Sender = new UserBase
					{
						Name = "PlayZoneBot"
					},
					Text = message.Text
				});
			}
		}

		public async Task ProcessMessage(CallbackData callbackData)
		{
			await ProcessMessageBase(callbackData.Sender.Id, callbackData.Sender.Name, "",
				((TextMessage)callbackData.Message).Text);
		}

		private static Keyboard GetButtons<T>(List<T> data)
		{
			var first = data.First();
			if (data.Count == 1 && !first.GetType().IsEnum)
			{
				var text = first.ToString();
				return new Keyboard
				{
					Buttons = new[]
					{
						new KeyboardButton
						{
							Text = text,
							ActionBody = text
						}
					}
				};
			}

			var structured = new List<KeyboardButton>();
			var step = data.Count > 3 ? 2 : 1;
			foreach (var value in data)
			{
				var text = value.GetDisplayName();
				structured.Add(new KeyboardButton
				{
					Text = text,
					ActionBody = text,
					Columns = 6 / step
				});
			}

			return new Keyboard
			{
				Buttons = structured.ToArray()
			};
		}
	}
}