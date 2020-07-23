using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.DTO;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace BusinessLayer.Helpers
{
	public static class BotClientHelper
	{
		public static async Task<Message> SendTextMessage(this TelegramBotClient client, AnswerMessageBase message)
		{
			Message tgMessage;
			/*if (string.IsNullOrEmpty(message.Text))
			{
				return null;
			}*/

			if (message.IsPhoto)
			{
				tgMessage = await client.SendPhotoAsync(message.UserId,
					message.Image != null
						? new InputOnlineFile(new MemoryStream(message.Image))
						: new InputOnlineFile(message.Text),
					replyMarkup: KeyboardHelper.GetKeyboardTelegram(message.Keyboard));
			}
			/*else if (message.Image != null)
			{
			    var iof = new InputOnlineFile(new MemoryStream(message.Image)) {FileName = "festival-map.jpg"};
			    await client.SendDocumentAsync(message.UserId, iof, "Карта фестиваля",
			        replyMarkup: KeyboardHelper.GetKeyboardTelegram(message.Keyboard));
			}*/
			else if (message.Keyboard != null)
			{
				tgMessage = await client.SendTextMessageAsync(message.UserId, message.Text,
					replyMarkup: KeyboardHelper.GetKeyboardTelegram(message.Keyboard, message.IsOneTimeKeyboard),
					parseMode: message.IsHtml ? ParseMode.Html : ParseMode.Default);
			}
			else if (message.KeyboardList != null)
			{
				tgMessage = await client.SendTextMessageAsync(message.UserId, message.Text,
					replyMarkup: KeyboardHelper.GetKeyboardTelegram(message.KeyboardList),
					parseMode: message.IsHtml ? ParseMode.Html : ParseMode.Default);
			}
			else if (message.InlineKeyboard != null)
			{
				var inlineKeyboard = new InlineKeyboardMarkup(message.InlineKeyboard
					.Select(d => new[] {InlineKeyboardButton.WithCallbackData(d.Key, d.Value)}).ToArray());
				tgMessage = await client.SendTextMessageAsync(message.UserId, message.Text,
					replyMarkup: inlineKeyboard, parseMode: message.IsHtml ? ParseMode.Html : ParseMode.Default);
			}
			else if (message.IsForceReplyMarkup)
			{
				tgMessage = await client.SendTextMessageAsync(message.UserId, message.Text,
					replyMarkup: new ForceReplyMarkup());
			}
			else
			{
				tgMessage = await client.SendTextMessageAsync(message.UserId, message.Text);
			}

			return tgMessage;
		}
	}
}