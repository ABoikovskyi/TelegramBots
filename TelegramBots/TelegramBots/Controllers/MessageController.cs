using System.IO;
using System.Threading.Tasks;
using BusinessLayer.Services.PlayZone;
using BusinessLayer.Services.PopCorn;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Viber.Bot;

namespace TelegramBots.Controllers
{
	[Route("api/message/playzoneupdate")]
	public class PlayZoneMessageController : Controller
	{
		private readonly PlayZoneBotServiceTelegram _botService;

		public PlayZoneMessageController(PlayZoneBotServiceTelegram botService)
		{
			_botService = botService;
		}

		[HttpGet]
		public string Get()
		{
			return "Method GET unuvalable";
		}

		[HttpPost]
		public async Task<OkResult> Post([FromBody] Update update)
		{
			if (update == null)
			{
				return Ok();
			}

			if (update.CallbackQuery != null)
			{
				await _botService.ProcessCallbackMessage(update.CallbackQuery);
			}
			else
			{
				await _botService.ProcessMessage(update.Message);
			}

			return Ok();
		}
	}
	
	[Route("api/message/playzoneviberupdate")]
	public class PlayZoneViberMessageController : Controller
	{
		private readonly PlayZoneBotServiceViber _botService;

		public PlayZoneViberMessageController(PlayZoneBotServiceViber botService)
		{
			_botService = botService;
		}

		[HttpGet]
		public string Get()
		{
			return "Method GET unuvalable";
		}

		[HttpPost]
		public async Task<OkResult> Post()
		{
			using (var reader = new StreamReader(Request.Body))
			{
				var body = reader.ReadToEnd();
				var callbackData = JsonConvert.DeserializeObject<CallbackData>(body);
				if (callbackData.Message is TextMessage)
				{
					await _botService.ProcessMessage(callbackData);
				}
			}

			return Ok();
		}
	}

	[Route("api/message/popcornupdate")]
	public class PopCornMessageController : Controller
	{
		private readonly PopCornBotServiceTelegram _botService;

		public PopCornMessageController(PopCornBotServiceTelegram botService)
		{
			_botService = botService;
		}

		[HttpGet]
		public string Get()
		{
			return "Method GET unuvalable";
		}

		[HttpPost]
		public async Task<OkResult> Post([FromBody] Update update)
		{
			if (update == null)
			{
				return Ok();
			}

			await _botService.ProcessMessage(update.Message);

			return Ok();
		}
	}

	[Route("api/message/popcornviberupdate")]
	public class PopCornViberMessageController : Controller
	{
		private readonly PopCornBotServiceViber _botService;

		public PopCornViberMessageController(PopCornBotServiceViber botService)
		{
			_botService = botService;
		}

		[HttpGet]
		public string Get()
		{
			return "Method GET unuvalable";
		}

		[HttpPost]
		public async Task<OkResult> Post()
		{
			using (var reader = new StreamReader(Request.Body))
			{
				var body = reader.ReadToEnd();
				var callbackData = JsonConvert.DeserializeObject<CallbackData>(body);
				if (callbackData.Message is TextMessage)
				{
					await _botService.ProcessMessage(callbackData);
				}
			}

			return Ok();
		}
	}
}