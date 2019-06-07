﻿using System.Threading.Tasks;
using BusinessLayer.Services.Festival;
using BusinessLayer.Services.NBCocktailsBar;
using BusinessLayer.Services.PlayZone;
using BusinessLayer.Services.PopCorn;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

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

	[Route("api/message/nbbarupdate")]
	public class NBBarMessageController : Controller
	{
		private readonly NBCocktailsBarBotServiceTelegram _botService;

		public NBBarMessageController(NBCocktailsBarBotServiceTelegram botService)
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

    [Route("api/message/festivalupdate")]
    public class FestivalMessageController : Controller
    {
        private readonly FestivalBotService _botService;

        public FestivalMessageController(FestivalBotService botService)
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
}