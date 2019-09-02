using System.Threading.Tasks;
using BusinessLayer.Services.Idrink;
using BusinessLayer.Services.OrangeClub;
using DataLayer.Context;
using Microsoft.AspNetCore.Mvc;

namespace TelegramBots.Controllers
{
	public class HomeController : Controller
	{
		private readonly IdrinkDbContext _context;
		private readonly OrangeClubService _orangeClubService;

		public HomeController(IdrinkDbContext context, OrangeClubService orangeClubService)
		{
			_context = context;
			_orangeClubService = orangeClubService;
		}

		public ActionResult Index()
		{
			return View();
		}

		/*[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
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
		}*/

		public async Task SetWebHooks()
		{
			/*await PlayZoneBotServiceTelegram.SetWebHook();
			await PlayZoneBotServiceViber.SetWebHook();
			await PopCornBotServiceTelegram.SetWebHook();
			await PopCornBotServiceViber.SetWebHook();
			await NBCocktailsBarBotServiceTelegram.SetWebHook();
			await FestivalBotService.SetWebHook();*/
			await IdrinkBotService.SetWebHook();
		}

		[HttpGet]
		public async Task<string> Get()
		{
			return "Method GET unavailable";
		}

		public string GetPostCode()
		{
			return _orangeClubService.GetPromoCode();
		}
	}
}