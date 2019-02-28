using System.Linq;
using System.Threading.Tasks;
using DataLayer.Context;
using DataLayer.Models.NBCocktailsBar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TelegramBots.Controllers
{
	public class NBCocktailsBarController : Controller
	{
		private readonly NBCocktailsBarDbContext _context;

		public NBCocktailsBarController(NBCocktailsBarDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult IngredientCategories()
		{
			return View(_context.IngredientCategories.OrderBy(f => f.Name).ToList());
		}
		
		public IActionResult IngredientCategory(int? id)
		{
			return View(id.HasValue ? _context.IngredientCategories.First(c => c.Id == id.Value) : new IngredientCategory());
		}

		public async Task<IActionResult> IngredientCategorySave(IngredientCategory data)
		{
			if (_context.IngredientCategories.Any(c => c.Id == data.Id))
			{
				_context.Update(data);
			}
			else
			{
				_context.Add(data);
			}

			await _context.SaveChangesAsync();

			return RedirectToAction("IngredientCategories");
		}

		public IActionResult Ingredients()
		{
			return View(_context.Ingredients.OrderBy(f => f.Name).ToList());
		}

		public IActionResult Ingredient(int? id)
		{
			return View(id.HasValue ? _context.Ingredients.First(c => c.Id == id.Value) : new Ingredient());
		}

		public async Task<IActionResult> IngredientSave(Ingredient data)
		{
			if (_context.Ingredients.Any(c => c.Id == data.Id))
			{
				_context.Update(data);
			}
			else
			{
				_context.Add(data);
			}

			await _context.SaveChangesAsync();

			return RedirectToAction("Ingredients");
		}
		
		public IActionResult Cocktails()
		{
			return View(_context.Cocktails.OrderBy(f => f.Name).ToList());
		}

		public IActionResult Cocktail(int? id)
		{
			return View(id.HasValue
				? _context.Cocktails.Include(c => c.Ingredients).ThenInclude(i => i.Ingredient)
					.First(c => c.Id == id.Value)
				: new Cocktail());
		}

		public async Task<IActionResult> CocktailSave(Cocktail data)
		{
			if (_context.Cocktails.Any(c => c.Id == data.Id))
			{
				_context.Update(data);
			}
			else
			{
				_context.Add(data);
			}

			await _context.SaveChangesAsync();

			return RedirectToAction("Cocktails");
		}
	}
}