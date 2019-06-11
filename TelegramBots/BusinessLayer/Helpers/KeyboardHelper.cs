using System.Collections.Generic;
using System.Linq;
using DataLayer.Helpers;
using Telegram.Bot.Types.ReplyMarkups;
using Viber.Bot;
using KeyboardButton = Telegram.Bot.Types.ReplyMarkups.KeyboardButton;

namespace BusinessLayer.Helpers
{
    public static class KeyboardHelper
    {
        public static ReplyKeyboardMarkup GetKeyboardTelegram<T>(List<T> data, bool isOneTimeKeyboard = false)
        {
            var first = data.First();
            var isEnum = first.GetType().IsEnum;
            if (data.Count == 1 && !isEnum)
            {
                return new ReplyKeyboardMarkup(new KeyboardButton(first.ToString()));
            }

            var structured = new List<KeyboardButton[]>();
            var step = data.Count > 3 ? 2 : 1;
            for (var i = 0; i < data.Count; i = i + step)
            {
                structured.Add(data.Skip(i).Take(step)
                    .Select(c => new KeyboardButton(isEnum ? c.GetDisplayName() : c.ToString())).ToArray());
            }

            return new ReplyKeyboardMarkup
            {
                Keyboard = structured.ToArray(),
                ResizeKeyboard = true,
                OneTimeKeyboard = isOneTimeKeyboard
            };
        }

        public static ReplyKeyboardMarkup GetKeyboardTelegram(string[][] keyboard, bool isOneTimeKeyboard = false)
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = keyboard.Select(k => k.Select(kk => (KeyboardButton)kk).ToArray()).ToArray(),
                ResizeKeyboard = true,
                OneTimeKeyboard = isOneTimeKeyboard
            };
        }
    }
}