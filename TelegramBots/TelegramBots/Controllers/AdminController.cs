using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Services.Idrink;
using DataLayer.Context;
using DataLayer.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
			return View(new IdrinkMessage());
		}

		[HttpPost]
		public async Task SendGlobalMessageWithDateCondition(IdrinkMessage message)
		{
			await _botService.SendGlobalMessageWithDateCondition(message);
		}
	}
}