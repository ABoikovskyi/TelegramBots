using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using TelegramBots.Services;

namespace TelegramBots.Controllers
{
	[Route("api/message/playzoneupdate")]
	public class PlayZoneMessageController : Controller
	{
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
				await PlayZoneBotService.ProcessCallbackMessage(update.CallbackQuery);
			}
			else
			{
				await PlayZoneBotService.ProcessMessage(update.Message);
			}

			return Ok();
		}
	}

	[Route("api/message/popcornupdate")]
	public class PopCornMessageController : Controller
	{
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
				await PopCornBotService.ProcessCallbackMessage(update.CallbackQuery);
			}
			else
			{
				await PopCornBotService.ProcessMessage(update.Message);
			}

			return Ok();
		}
	}
}