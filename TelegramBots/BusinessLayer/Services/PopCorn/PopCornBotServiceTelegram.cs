using System;
using System.Threading.Tasks;
using BusinessLayer.Helpers;
using DataLayer.Context;
using DataLayer.Models.DTO;
using DataLayer.Models.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BusinessLayer.Services.PopCorn
{
	public class PopCornBotServiceTelegram : PopCornBotServiceBase
	{
		public static TelegramBotClient Client;

		public PopCornBotServiceTelegram(PopCornDbContext context, MemoryCacheHelper memoryCacheHelper) : base(context,
			memoryCacheHelper)
		{
		}

		public static void Init()
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
			await ProcessMessageBase(message.Chat.Id, Messenger.Telegram, message.Chat.FirstName, message.Chat.LastName,
				message.Text);
		}
	}
}