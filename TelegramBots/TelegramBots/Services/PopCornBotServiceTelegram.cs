using System;
using System.Threading.Tasks;
using DataLayer.Models.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBots.Context;
using TelegramBots.Helpers;
using TelegramBots.Models;

namespace TelegramBots.Services
{
	public class PopCornBotServiceTelegram : PopCornBotServiceBase
	{
		public static TelegramBotClient Client;
		public PopCornBotServiceTelegram(PopCornDbContext context) : base(context)
		{
		}

		public static void Init(IServiceProvider serviceProvider)
		{
			if (Client == null)
			{
				Client = new TelegramBotClient("723676644:AAE9j7lkkUdGnef3JMerwC6hHXVLkccdyLk");
			}
		}

		public static async Task SetWebHook()
		{
			await Client.DeleteWebhookAsync();
			await Client.SetWebhookAsync($"{Links.AppLink}/api/message/popcornupdate");
		}
		
		public override async Task SendTextMessage(AnswerMessageBase message)
		{
			await Client.SendTextMessage(message);
		}

		public async Task ProcessMessage(Message message)
		{
			await ProcessMessageBase(message.Chat.Id.ToString(), Messenger.Telegram, message.Chat.FirstName, message.Chat.LastName,
				message.Text);
		}
	}
}