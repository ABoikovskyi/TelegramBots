using System.Linq;
using System.Threading.Tasks;
using DataLayer.Context;
using DataLayer.Models.NBCocktailsBar;
using Microsoft.AspNetCore.Mvc;

namespace TelegramBots.Controllers
{
	public class NBCocktailsBarController : Controller
	{
		private readonly NBCocktailsBarDbContext _context;

		public NBCocktailsBarController(NBCocktailsBarDbContext context)
		{
			_context = context;
		}

		public IActionResult Flavors()
		{
			return View(_context.Flavors.OrderBy(f => f.Id).ToList());
		}

		public IActionResult Flavor(int? id)
		{
			return View(id.HasValue ? _context.Flavors.First(c => c.Id == id.Value) : new Flavor());
		}

		public async Task<IActionResult> FlavorSave(Flavor data)
		{
			if (_context.Flavors.Any(c => c.Id == data.Id))
			{
				_context.Update(data);
			}
			else
			{
				_context.Add(data);
			}

			await _context.SaveChangesAsync();

			return RedirectToAction("Flavors");
		}

		public IActionResult AlcoholDrinks()
		{
			return View(_context.AlcoholDrinks.OrderBy(f => f.Id).ToList());
		}

		public IActionResult AlcoholDrink(int? id)
		{
			return View(id.HasValue ? _context.AlcoholDrinks.First(c => c.Id == id.Value) : new AlcoholDrink());
		}

		public async Task<IActionResult> AlcoholDrinkSave(AlcoholDrink data)
		{
			if (_context.AlcoholDrinks.Any(c => c.Id == data.Id))
			{
				_context.Update(data);
			}
			else
			{
				_context.Add(data);
			}

			await _context.SaveChangesAsync();

			return RedirectToAction("AlcoholDrinks");
		}

		public IActionResult ABVs()
		{
			return View(_context.ABVs.OrderBy(f => f.Id).ToList());
		}

		public IActionResult ABV(int? id)
		{
			return View(id.HasValue ? _context.ABVs.First(c => c.Id == id.Value) : new ABV());
		}

		public async Task<IActionResult> ABVSave(ABV data)
		{
			if (_context.ABVs.Any(c => c.Id == data.Id))
			{
				_context.Update(data);
			}
			else
			{
				_context.Add(data);
			}

			await _context.SaveChangesAsync();

			return RedirectToAction("ABVs");
		}
	}
}