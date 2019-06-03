using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Context;
using DataLayer.Models.Festival;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TelegramBots.Controllers
{
	public class FestivalController : Controller
	{
		private readonly FestivalDbContext _context;

		public FestivalController(FestivalDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Festivals()
		{
			return View(_context.Festivals.OrderBy(f => f.StartDate).ToList());
		}
		
		public IActionResult Festival(int? id)
		{
			return View(id.HasValue ? _context.Festivals.First(c => c.Id == id.Value) : new Festival());
		}

		public async Task<IActionResult> FestivalSave(Festival data)
        {
            foreach (var file in Request.Form.Files)
            {
                if (file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        data.Map = ms.ToArray();
                    }
                }
            }

            if (_context.Festivals.Any(c => c.Id == data.Id))
			{
				_context.Update(data);
			}
			else
			{
				_context.Add(data);
			}

			await _context.SaveChangesAsync();

			return RedirectToAction("Festivals");
		}

		public IActionResult Stages()
        {
            return View(_context.Stages.Include(i => i.Festival)
                .GroupBy(i => i.Festival).OrderBy(i => i.Key)
                .ToDictionary(g => g.Key.Name, g => g.OrderBy(i => i.Name).ToList()));
        }

		public IActionResult Stage(int? id)
		{
			ViewBag.Events = _context.Festivals.OrderBy(c => c.Id).ToList();
			return View(id.HasValue ? _context.Stages.First(c => c.Id == id.Value) : new Stage());
		}

		public async Task<IActionResult> StageSave(Stage data)
		{
			if (_context.Stages.Any(c => c.Id == data.Id))
			{
				_context.Update(data);
			}
			else
			{
				_context.Add(data);
			}

			await _context.SaveChangesAsync();

			return RedirectToAction("Stages");
		}
		
		public IActionResult Artists()
		{
			return View(_context.Artists.OrderBy(f => f.Name).ToList());
		}

		public IActionResult Artist(int? id)
        {
            return View(id.HasValue ? _context.Artists.First(c => c.Id == id.Value) : new Artist());
        }

		public async Task<IActionResult> ArtistSave(Artist data)
        {
            foreach (var file in Request.Form.Files)
            {
                if (file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        data.Image = ms.ToArray();
                    }
                }
            }

            if (_context.Artists.Any(c => c.Id == data.Id))
            {
                _context.Update(data);
            }
            else
            {
                _context.Add(data);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Artists");
        }
	}
}