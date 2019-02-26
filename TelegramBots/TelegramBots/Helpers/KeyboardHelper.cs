using System.Collections.Generic;
using System.Linq;
using DataLayer.Helpers;
using Telegram.Bot.Types.ReplyMarkups;
using Viber.Bot;
using KeyboardButton = Telegram.Bot.Types.ReplyMarkups.KeyboardButton;

namespace TelegramBots.Helpers
{
	public static class KeyboardHelper
	{
		public static ReplyKeyboardMarkup GetKeyboardTelegram<T>(List<T> data)
		{
			var first = data.First();
			if (data.Count == 1 && !first.GetType().IsEnum)
			{
				return new ReplyKeyboardMarkup(new KeyboardButton(first.ToString()));
			}

			var structured = new List<KeyboardButton[]>();
			var step = data.Count > 3 ? 2 : 1;
			for (var i = 0; i < data.Count; i = i + step)
			{
				structured.Add(data.Skip(i).Take(step)
					.Select(c => new KeyboardButton(c.GetDisplayName())).ToArray());
			}

			return new ReplyKeyboardMarkup
			{
				Keyboard = structured.ToArray(),
				ResizeKeyboard = true
			};
		}

		public static ReplyKeyboardMarkup GetKeyboardTelegram(string[][] keyboard)
		{
			return new ReplyKeyboardMarkup
			{
				Keyboard = keyboard.Select(k => k.Select(kk => (KeyboardButton)kk).ToArray()).ToArray(),
				ResizeKeyboard = true
			};
		}

		public static Keyboard GetKeyboardViber<T>(List<T> data)
		{
			var first = data.First();
			if (data.Count == 1 && !first.GetType().IsEnum)
			{
				var text = first.ToString();
				return new Keyboard
				{
					Buttons = new[]
					{
						new Viber.Bot.KeyboardButton
						{
							Text = text,
							ActionBody = text
						}
					}
				};
			}

			var structured = new List<Viber.Bot.KeyboardButton>();
			var step = data.Count > 3 ? 2 : 1;
			foreach (var value in data)
			{
				var text = value.GetDisplayName();
				structured.Add(new Viber.Bot.KeyboardButton
				{
					Text = text,
					ActionBody = text,
					Columns = 6 / step
				});
			}

			return new Keyboard
			{
				Buttons = structured.ToArray()
			};
		}

		public static Keyboard GetKeyboardViber(string[][] keyboard)
		{
			return new Keyboard
			{
				Buttons = keyboard.SelectMany(k => k.Select(kk => new Viber.Bot.KeyboardButton
				{
					Text = kk,
					ActionBody = kk,
					Columns = 6 / k.Length
				}).ToArray()).ToArray()
			};
		}
	}
}