﻿using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.DTO;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace BusinessLayer.Helpers
{
	public static class BotClientHelper
	{
        public static async Task SendTextMessage(this TelegramBotClient client, AnswerMessageBase message)
        {
            if (string.IsNullOrEmpty(message.Text))
            {
                return;
            }

            if (message.IsHtml)
            {
                await client.SendTextMessageAsync(message.UserId, message.Text, ParseMode.Html,
                    replyMarkup: KeyboardHelper.GetKeyboardTelegram(message.Keyboard), disableWebPagePreview: true);
            }
            else if (message.IsPhoto)
            {
                await client.SendPhotoAsync(message.UserId,
                    message.Image != null
                        ? new InputOnlineFile(new MemoryStream(message.Image))
                        : new InputOnlineFile(message.Text),
                    replyMarkup: KeyboardHelper.GetKeyboardTelegram(message.Keyboard));
            }
            else if (message.Image != null)
            {
                var iof = new InputOnlineFile(new MemoryStream(message.Image)) {FileName = "festival-map.jpg"};
                await client.SendDocumentAsync(message.UserId, iof, "Карта фестиваля",
                    replyMarkup: KeyboardHelper.GetKeyboardTelegram(message.Keyboard));
            }
            else if (message.Keyboard != null)
            {
                await client.SendTextMessageAsync(message.UserId, message.Text,
                    replyMarkup: KeyboardHelper.GetKeyboardTelegram(message.Keyboard, message.IsOneTimeKeyboard));
            }
            else if (message.KeyboardList != null)
            {
                await client.SendTextMessageAsync(message.UserId, message.Text,
                    replyMarkup: KeyboardHelper.GetKeyboardTelegram(message.KeyboardList));
            }
            else if (message.InlineKeyboard != null)
            {
                var inlineKeyboard = new InlineKeyboardMarkup(message.InlineKeyboard
                    .Select(d => new[] {InlineKeyboardButton.WithCallbackData(d.Key, d.Value)}).ToArray());
                await client.SendTextMessageAsync(message.UserId, message.Text,
                    replyMarkup: inlineKeyboard);
            }
            else if (message.IsForceReplyMarkup)
            {
                await client.SendTextMessageAsync(message.UserId, message.Text,
                    replyMarkup: new ForceReplyMarkup());
            }
            else
            {
                await client.SendTextMessageAsync(message.UserId, message.Text);
            }
        }
	}
}