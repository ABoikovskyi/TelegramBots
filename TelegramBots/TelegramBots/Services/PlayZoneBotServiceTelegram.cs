using System;
using System.Threading.Tasks;
using DataLayer.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBots.Context;
using TelegramBots.Helpers;
using TelegramBots.Models;

namespace TelegramBots.Services
{
	public class PlayZoneBotServiceTelegram : PlayZoneBotServiceBase
	{
		public static TelegramBotClient Client;
		public PlayZoneBotServiceTelegram(PlayZoneDbContext context) : base(context)
		{
		}

		public static void Init(IServiceProvider serviceProvider)
		{
			if (Client == null)
			{
				Client = new TelegramBotClient("626205769:AAFc8pYf1QMiF0zQQQKbAJOz038VsL6T1aQ");
			}
		}

		public static async Task SetWebHook()
		{
			await Client.DeleteWebhookAsync();
			await Client.SetWebhookAsync($"{Links.AppLink}/api/message/playzoneupdate");
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
			await Client.SendTextMessage(message);
		}

		public async Task ProcessMessage(Message message)
		{
			await ProcessMessageBase(message.Chat.Id.ToString(), message.Chat.FirstName, message.Chat.LastName,
				message.Text);
		}
	}
}