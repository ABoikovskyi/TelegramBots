namespace BusinessLayer.Helpers
{
	public class Links
	{
#if DEBUG
		public const string AppLink = "https://b81004a9.ngrok.io";
#else
		public const string AppLink = "https://idrink.com.ua";
#endif

#if DEBUG
		public const string TelegramKey = "948300668:AAEmJVBENIytmAGx78wJ53dqwcOMAFoUK1Y";
#else
		public const string TelegramKey = "740230403:AAEY_iB-EA_v63uiTXj7ohEq2rgqbqxeojc";
#endif
	}
}