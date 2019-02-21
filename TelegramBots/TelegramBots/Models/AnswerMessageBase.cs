﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TelegramBots.Models
{
	public class AnswerMessageBase
	{
		public AnswerMessageBase(string userId, string text)
		{
			UserId = userId;
			Text = text;
		}

		public AnswerMessageBase(string userId, string text, Type keyboardEnum)
		{
			UserId = userId;
			Text = text;
			KeyboardList = Enum.GetValues(keyboardEnum).Cast<object>().ToList();
		}

		public AnswerMessageBase(string userId, string text, List<object> keyboardList)
		{
			UserId = userId;
			Text = text;
			KeyboardList = keyboardList;
		}

		public string UserId { get; set; }
		public string Text { get; set; }
		public List<object> KeyboardList { get; set; }
		public bool IsForceReplyMarkup { get; set; }
		public Dictionary<string, string> InlineKeyboard { get; set; }
	}
}