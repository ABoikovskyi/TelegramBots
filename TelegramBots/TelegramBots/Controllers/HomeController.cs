using System.Diagnostics;
using System.Linq;
using DataLayer.Models;
using DataLayer.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using TelegramBots.Context;
using TelegramBots.Models;

namespace TelegramBots.Controllers
{
	public class HomeController : Controller
	{
		private readonly PlayZoneDbContext _context;

		public HomeController(PlayZoneDbContext context)
		{
			_context = context;
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		public ActionResult Index()
		{
			ViewBag.HideMenu = true;
			return View(_context.UserRequests.ToList());
		}

		public ActionResult Requests()
		{
			ViewBag.HideMenu = true;
			return View(_context.UserRequests.ToList());
		}

		public ActionResult UpdateStatus(int id, RequestStatus status)
		{
			var request = _context.UserRequests.First(r => r.Id == id);
			request.Status = status;
			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		public ActionResult EditRequest(int id)
		{
			ViewBag.HideMenu = true;
			return View(_context.UserRequests.First(r => r.Id == id));
		}

		[HttpPost]
		public ActionResult RequestSave(UserRequest data)
		{
			_context.Update(data);
			_context.SaveChanges();

			return RedirectToAction("EditRequest", new { id = data.Id });
		}
	}
}
