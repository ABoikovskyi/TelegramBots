using System;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Services;
using BusinessLayer.Services.PopCorn;
using DataLayer.Context;
using DataLayer.Models.Enums;
using DataLayer.Models.PopCorn;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TelegramBots.Controllers
{
	[Authorize]
	public class PopCornController : Controller
	{
		private readonly PopCornDbContext _context;
		private readonly ExportService _exportService;
		private readonly PopCornBotServiceTelegram _telegramBotService;
		private readonly PopCornBotServiceViber _viberBotService;

		public PopCornController(PopCornDbContext context, ExportService exportService, 
			PopCornBotServiceTelegram telegramBotService, PopCornBotServiceViber viberBotService)
		{
			_context = context;
			_exportService = exportService;
			_telegramBotService = telegramBotService;
			_viberBotService = viberBotService;
		}

		public IActionResult StartPage()
		{
			return View("Index");
		}
		
		public IActionResult MainInfo()
		{
			return View(_context.MainInfo.First());
		}

		public IActionResult MainInfoSave(MainInfo data)
		{
			_context.Update(data);
			_context.SaveChanges();
			//MemoryCacheHelper.SetMainInfo(data);

			return RedirectToAction("MainInfo");
		}

		public IActionResult Concerts()
		{
			return View(_context.Concerts.OrderByDescending(c => c.EventDate)
				.Select(c => new ShortConcertInfo {Id = c.Id, Title = c.Artist, EventDate = c.EventDate}).ToList());
		}

		public IActionResult Concert(int? id)
		{
			return View(id.HasValue ? _context.Concerts.First(c => c.Id == id.Value) : new Concert());
		}

		public async Task<IActionResult> ConcertSave(Concert data)
		{
			if (_context.Concerts.Any(c => c.Id == data.Id))
			{
				_context.Update(data);
			}
			else
			{
				_context.Add(data);
			}

			await _context.SaveChangesAsync();

			return RedirectToAction("Concerts");
		}

		public IActionResult Posts()
		{
			return View(_context.Posts.Include(n => n.Concert).ToList());
		}

		public IActionResult Post(int? id)
		{
			ViewBag.Concerts = _context.Concerts.Select(c => new {c.Id, c.Artist})
				.ToDictionary(c => c.Id, c => c.Artist);
			return View(id.HasValue ? _context.Posts.First(c => c.Id == id.Value) : new Post());
		}

		public async Task<IActionResult> PostSave(Post data)
		{
			var postData = _context.Posts.FirstOrDefault(c => c.Id == data.Id);
			if (postData != null)
			{
				if (data.ScheduleDate.HasValue)
				{
					data.Status = PostStatus.Scheduled;
					if (!postData.ScheduleDate.HasValue)
					{
						await QuartzService.StartPostPublisherJob(data.Id, data.ScheduleDate.Value);
					}
					else if (postData.ScheduleDate != data.ScheduleDate)
					{
						await QuartzService.DeletePostPublishJob(data.Id);
						await QuartzService.StartPostPublisherJob(data.Id, data.ScheduleDate.Value);
					}
				}
				
				_context.Update(data);
				await _context.SaveChangesAsync();
			}
			else
			{
				data.Status = PostStatus.Created;
				data.Date = DateTime.Now;

				if (data.ScheduleDate.HasValue)
				{
					data.Status = PostStatus.Scheduled;
				}
				
				_context.Add(data);
				await _context.SaveChangesAsync();

                if (data.ScheduleDate.HasValue)
                {
                    await QuartzService.StartPostPublisherJob(data.Id, data.ScheduleDate.Value);
                }
			}

			return RedirectToAction("Post", "PopCorn", new {id = data.Id});
		}

		public async Task<IActionResult> PublishPost(int postId)
		{
			var post = _context.Posts.Include(p => p.Concert).FirstOrDefault(p => p.Id == postId);
			if (post != null && post.Status != PostStatus.Published)
			{
				await _telegramBotService.SendNewPostAlert(post, Messenger.Telegram);
				await _viberBotService.SendNewPostAlert(post, Messenger.Viber);
				if (post.Status == PostStatus.Scheduled)
				{
					await QuartzService.DeletePostPublishJob(postId);
				}

				post.Status = PostStatus.Published;
				post.PublishDate = DateTime.Now;
				_context.Update(post);
				_context.SaveChanges();
			}

			return RedirectToAction("Posts");
		}

		public ActionResult GetUsersReport()
		{
			using (var ms = _exportService.GetUsersReport())
			{
				return File(ms.ToArray(), System.Net.Mime.MediaTypeNames.Application.Octet,
					$"UsersReport_{DateTime.UtcNow}.xlsx");
			}
		}

		public ActionResult ConcertUaData()
		{
			return View("ConcertUaData");
		}

		public async Task<ActionResult> GetConcertUaData(string url)
		{
			ViewBag.Url = url;
			return View("ConcertUaData", await ConcertUaService.GetProcessedData(url));
		}
	}
}