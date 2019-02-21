using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBots.Models;

namespace TelegramBots.Services
{
	public class PlayZoneBotServiceTelegram : PlayZoneBotServiceBase
	{
		public static TelegramBotClient Client;

		public new const string AppLink = "https://playzone.ua";
		//public new const string AppLink = "https://84b65c6d.ngrok.io";

		public static void Init(IServiceProvider serviceProvider)
		{
			if (Client == null)
			{
				ServiceProvider = serviceProvider;
				Client = new TelegramBotClient("626205769:AAFc8pYf1QMiF0zQQQKbAJOz038VsL6T1aQ");
			}
		}

		public static async Task SetWebHook()
		{
			await Client.DeleteWebhookAsync();
			await Client.SetWebhookAsync($"{AppLink}/api/message/playzoneupdate");
		}

		public async Task ProcessCallbackMessage(CallbackQuery callback)
		{
			var message = callback.Message;
			var status = ProcessCallbackMessageBase(callback.Data, message.Chat.Id, message.MessageId, message.Text);
			if (status != null)
			{
				await Client.EditMessageTextAsync(message.Chat.Id, message.MessageId,
					message.Text + $"\r\n\r\nЗАЯВКА {status.GetDisplayName().ToUpper()}");
			}
		}

		public override async Task SendTextMessage(AnswerMessageBase message)
		{
			if (message.KeyboardList != null)
			{
				await Client.SendTextMessageAsync(message.UserId, message.Text,
					replyMarkup: GetButtons(message.KeyboardList));
			}
			else if (message.InlineKeyboard != null)
			{
				var inlineKeyboard = new InlineKeyboardMarkup(message.InlineKeyboard
					.Select(d => InlineKeyboardButton.WithCallbackData(d.Key, d.Value)).ToArray());
				await Client.SendTextMessageAsync(message.UserId, message.Text,
					replyMarkup: inlineKeyboard);
			}
			else if (message.IsForceReplyMarkup)
			{
				await Client.SendTextMessageAsync(message.UserId, message.Text,
					replyMarkup: new ForceReplyMarkup());
			}
			else
			{
				await Client.SendTextMessageAsync(message.UserId, message.Text);
			}
		}

		public async Task ProcessMessage(Message message)
		{
			await ProcessMessageBase(message.Chat.Id.ToString(), message.Chat.FirstName, message.Chat.LastName,
				message.Text);
		}

		private static ReplyKeyboardMarkup GetButtons<T>(List<T> data)
		{
			var first = data.First();
			if (data.Count == 1 && !first.GetType().IsEnum)
			{
				return new ReplyKeyboardMarkup(new KeyboardButton(first.ToString()));
			}

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