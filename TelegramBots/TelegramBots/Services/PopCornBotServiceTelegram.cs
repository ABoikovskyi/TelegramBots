using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBots.Helpers;
using TelegramBots.Models;

namespace TelegramBots.Services
{
	public class PopCornBotServiceTelegram : PopCornBotServiceBase
	{
		public static TelegramBotClient Client;
		
		public static void Init(IServiceProvider serviceProvider)
		{
			if (Client == null)
			{
				ServiceProvider = serviceProvider;
				Client = new TelegramBotClient("723676644:AAE9j7lkkUdGnef3JMerwC6hHXVLkccdyLk");
			}
		}

		public static async Task SetWebHook()
		{
			await Client.DeleteWebhookAsync();
			await Client.SetWebhookAsync($"{Links.AppLink}/api/message/popcornupdate");
		}

		public async Task ProcessCallbackMessage(CallbackQuery callback)
		{
			var message = callback.Message;
			var reply = ProcessCallbackMessageBase(callback.Data, message.Chat.Id, message.MessageId, message.Text);
			if (!string.IsNullOrEmpty(reply))
			{
				await Client.SendTextMessageAsync(message.Chat.Id, reply,
					replyMarkup: KeyboardHelper.GetKeyboardTelegram(MainKeyboard));
			}

			message.Text = callback.Data;
			await ProcessMessage(message);
		}

		public override async Task SendTextMessage(AnswerMessageBase message)
		{
			await Client.SendTextMessage(message);
		}

		public async Task ProcessMessage(Message message)
		{
			await ProcessMessageBase(message.Chat.Id.ToString(), message.Chat.FirstName, message.Chat.LastName,
				message.Text);
		}
	}
}