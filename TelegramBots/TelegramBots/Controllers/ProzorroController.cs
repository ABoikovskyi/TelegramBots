using System.Threading.Tasks;
using BusinessLayer.Services.Prozorro;
using Microsoft.AspNetCore.Mvc;

namespace TelegramBots.Controllers
{
	public class ProzorroController : Controller
	{
		private readonly ProzorroBotService _prozorroBotService;

		public ProzorroController(ProzorroBotService prozorroBotService)
		{
			_prozorroBotService = prozorroBotService;
		}

		public async Task Monitoring()
		{
			await _prozorroBotService.Monitoring();
		}
	}
}