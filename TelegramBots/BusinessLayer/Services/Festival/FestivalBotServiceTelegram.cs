using System.Threading.Tasks;
using BusinessLayer.Helpers;
using DataLayer.Context;
using DataLayer.Models.DTO;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BusinessLayer.Services.Festival
{
	public class FestivalBotServiceTelegram : FestivalBotServiceBase
    {
        public static TelegramBotClient Client;

        public FestivalBotServiceTelegram(FestivalDbContext context, MemoryCacheHelper memoryCacheHelper) : base(context,
            memoryCacheHelper)
        {
        }

        public static void Init()
        {
            if (Client == null)
            {
                Client = new TelegramBotClient("767658547:AAGHxb3XWihezv02gflFhy542ZclQR9HwA4");
            }
        }

        public static async Task SetWebHook()
        {
            await Client.DeleteWebhookAsync();
            await Client.SetWebhookAsync($"{Links.AppLink}/api/message/festivalupdate");
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