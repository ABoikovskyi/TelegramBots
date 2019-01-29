using System;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelegramBots.Context;
using TelegramBots.Helpers;
using TelegramBots.Services;

namespace TelegramBots.Controllers
{
	public class PopCornController : Controller
	{
		private readonly PopCornDbContext _context;
		private readonly ExportService _exportService;

		public PopCornController(PopCornDbContext context, ExportService exportService)
		{
			_context = context;
			_exportService = exportService;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult MainInfo()
		{
			return View(_context.MainInfo.First());
		}

		public IActionResult MainInfoSave(MainInfo data)
		{
			_context.Update(data);
			_context.SaveChanges();
			MemoryCacheHelper.SetMemoryInfo(data);

			return RedirectToAction("MainInfo");
		}

		public IActionResult Concerts()
		{
			return View(_context.Concerts.OrderByDescending(c => c.EventDate)
				.Select(c => new ShortConcertInfo { Id = c.Id, Title = c.Artist, EventDate = c.EventDate }).ToList());
		}

		public IActionResult Concert(int? id)
		{
			return View(id.HasValue ? _context.Concerts.First(c => c.Id == id.Value) : new Concert());
		}

		public async Task<IActionResult> ConcertSave(Concert data)
		{
			var concerts = MemoryCacheHelper.GetConcerts();
			var concertData = concerts.FirstOrDefault(c => c.Id == data.Id);

			if (concertData != null)
			{
				concertData = data;
				_context.Update(concertData);
			}
			else
			{
				concerts.Add(data);
				_context.Add(data);
			}

			await _context.SaveChangesAsync();
			MemoryCacheHelper.SetConcerts(concerts);

			return RedirectToAction("Concerts");
		}

		public IActionResult News()
		{
			return View(_context.News.Include(n => n.Concert).ToList());
		}

		public IActionResult Post(int? id)
		{
			ViewBag.Concerts = _context.Concerts.Select(c => new { c.Id, c.Artist })
				.ToDictionary(c => c.Id, c => c.Artist);
			return View(id.HasValue ? _context.News.First(c => c.Id == id.Value) : new News());
		}

		public async Task<IActionResult> PostSave(News data)
		{
			var news = MemoryCacheHelper.GetNews();
			var postData = news.FirstOrDefault(c => c.Id == data.Id);

			if (postData != null)
			{
				postData = data;
				_context.Update(postData);
			}
			else
			{
				data.Date = DateTime.Now;
				news.Add(data);
				_context.Add(data);
			}

			await _context.SaveChangesAsync();
			MemoryCacheHelper.SetNews(news);

			return RedirectToAction("Post", "PopCorn", new { id = data.Id });
		}

		public async Task<IActionResult> PublishPost(int postId)
		{
			var post = _context.News.First(p => p.Id == postId);
			await PopCornBotService.SendNewPostAlert(post);

			post.IsPublished = true;
			_context.Update(post);
			_context.SaveChanges();

			return RedirectToAction("News");
		}

		public ActionResult GetUsersReport()
		{
			using (var ms = _exportService.GetUsersReport())
			{
				return File(ms.ToArray(), System.Net.Mime.MediaTypeNames.Application.Octet,
					$"UsersReport_{DateTime.UtcNow}.xlsx");
			}
		}
	}
}

