﻿using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Services.Idrink;
using DataLayer.Context;
using DataLayer.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TelegramBots.Controllers
{
	[Authorize]
    public class AdminController : Controller
    {
	    private readonly IdrinkBotService _botService;
		private readonly IdrinkDbContext _repository;

	    public AdminController(IdrinkDbContext repository, IdrinkBotService botService)
		{
		    _repository = repository;
		    _botService = botService;
		}

		public IActionResult Users()
        {
            return View(_repository.Users.ToList());
		}

		public IActionResult Log()
		{
			return View(_repository.Log.ToList());
		}

		public IActionResult GlobalMessageWithDateCondition()
		{
			ViewBag.Users = _repository.Users.Select(c =>
					new SelectListItem {Value = c.Id.ToString(), Text = $"{c.FirstName} - {c.LastName} ({c.UserName})"})
				.ToList();
			return View(new IdrinkMessage());
		}

		[HttpPost]
		public async Task<string> SendGlobalMessageWithDateCondition(IdrinkMessage message)
		{
			return await _botService.SendGlobalMessageWithDateCondition(message);
		}
	}
}