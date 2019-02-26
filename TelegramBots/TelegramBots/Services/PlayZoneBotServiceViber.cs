using System;
using System.Threading.Tasks;
using TelegramBots.Context;
using TelegramBots.Helpers;
using TelegramBots.Models;
using Viber.Bot;

namespace TelegramBots.Services
{
	public class PlayZoneBotServiceViber : PlayZoneBotServiceBase
	{
		public static ViberBotClient Client;
		public static string ViberAdminId = "+EevijqQxF1RlnZCJvQsyQ==";
		public PlayZoneBotServiceViber(PlayZoneDbContext context) : base(context)
		{
		}

		public static void Init(IServiceProvider serviceProvider)
		{
			if (Client == null)
			{
				Client = new ViberBotClient("494050bd2f67d427-b0167b87f79c3f8e-323999acbc7b6097");
			}
		}

		public static async Task SetWebHook()
		{
			await Client.SetWebhookAsync("");
			await Client.SetWebhookAsync($"{Links.AppLink}/api/message/playzoneviberupdate");
		}

		public override async Task SendTextMessage(AnswerMessageBase message)
		{
			await Client.SendTextMessage(message, "PopCornBot");
		}

		public async Task ProcessMessage(CallbackData callbackData)
		{
			await ProcessMessageBase(callbackData.Sender.Id, callbackData.Sender.Name, "",
				((TextMessage)callbackData.Message).Text);
		}
	}
}