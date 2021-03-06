﻿using System.Threading.Tasks;
using BusinessLayer.Helpers;
using DataLayer.Context;
using DataLayer.Models.DTO;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BusinessLayer.Services.NBCocktailsBar
{
	public class NBCocktailsBarBotServiceTelegram : NBCocktailsBarBotServiceBase
    {
		public static TelegramBotClient Client;

		public NBCocktailsBarBotServiceTelegram(NBCocktailsBarDbContext context, MemoryCacheHelper memoryCacheHelper) :
			base(context, memoryCacheHelper)
		{
		}

		public static void Init()
		{
			if (Client == null)
			{
				Client = new TelegramBotClient("792424451:AAG5a28HxzD0hjBcXDktfbUyINGK8XrJmSU");
			}
		}

		public static async Task SetWebHook()
		{
			await Client.DeleteWebhookAsync();
			await Client.SetWebhookAsync($"{Links.AppLink}/api/message/nbbarupdate");
		}
		
		public override async Task SendTextMessage(AnswerMessageBase message)
		{
			await SendTextMessageStatic(message);
		}

		public static async Task SendTextMessageStatic(AnswerMessageBase message)
		{
			await Client.SendTextMessage(message);
		}

		public async Task ProcessMessage(Message message)
		{
			await ProcessMessageBase(message.Chat.Id, message.Chat.FirstName, message.Chat.LastName,
				message.Text);
		}
	}
}