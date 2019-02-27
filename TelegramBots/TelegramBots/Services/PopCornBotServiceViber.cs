using System;
using System.Threading.Tasks;
using DataLayer.Models.Enums;
using TelegramBots.Context;
using TelegramBots.Helpers;
using TelegramBots.Models;
using Viber.Bot;

namespace TelegramBots.Services
{
	public class PopCornBotServiceViber : PopCornBotServiceBase
	{
		public static ViberBotClient Client;
		public PopCornBotServiceViber(PopCornDbContext context) : base(context)
		{
		}

		public static void Init(IServiceProvider serviceProvider)
		{
			if (Client == null)
			{
				Client = new ViberBotClient("494a81ade2e7d506-cf17f21b30373b9f-c66fee43f597e9b");
			}
		}

		public static async Task SetWebHook()
		{
			await Client.SetWebhookAsync("");
			await Client.SetWebhookAsync($"{Links.AppLink}/api/message/popcornviberupdate");
		}

		public override async Task SendTextMessage(AnswerMessageBase message)
		{
			await Client.SendTextMessage(message, "PlayZoneBot");
		}

		public async Task ProcessMessage(CallbackData callbackData)
		{
			await ProcessMessageBase(callbackData.Sender.Id, Messenger.Viber, callbackData.Sender.Name, "",
				((TextMessage)callbackData.Message).Text);
		}
	}
}