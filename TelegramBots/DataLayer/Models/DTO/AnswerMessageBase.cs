using System;
using System.Collections.Generic;
using System.Linq;

namespace DataLayer.Models.DTO
{
	public class AnswerMessageBase
	{
		public AnswerMessageBase(long userId)
		{
			UserId = userId;
		}

		public AnswerMessageBase(long userId, string text)
		{
			UserId = userId;
			Text = text;
		}

		public AnswerMessageBase(long userId, string text, Type keyboardEnum)
		{
			UserId = userId;
			Text = text;
			KeyboardList = Enum.GetValues(keyboardEnum).Cast<object>().ToList();
		}

		public AnswerMessageBase(long userId, string text, List<object> keyboardList)
		{
			UserId = userId;
			Text = text;
			KeyboardList = keyboardList;
        }

        public AnswerMessageBase(long userId, string text, string[][] keyboard)
		{
			UserId = userId;
			Text = text;
			Keyboard = keyboard;
        }

        public AnswerMessageBase(long userId, string text, byte[] image, string[][] keyboard)
        {
            UserId = userId;
            Image = image;
            Text = text;
            Keyboard = keyboard;
        }

        public long UserId { get; set; }
		public string Text { get; set; }
        public byte[] Image { get; set; }
        public bool IsPhoto { get; set; }
		public List<object> KeyboardList { get; set; }
		public string[][] Keyboard { get; set; }
		public bool IsForceReplyMarkup { get; set; }
        public bool IsOneTimeKeyboard { get; set; }
        public Dictionary<string, string> InlineKeyboard { get; set; }
		public bool IsHtml { get; set; }
	}
}