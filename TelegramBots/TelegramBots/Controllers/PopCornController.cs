using System;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models;
using DataLayer.Models.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelegramBots.Context;
using TelegramBots.Helpers;
using TelegramBots.Services;

namespace TelegramBots.Controllers
{
	public class PopCornController : Controller
	{
		private readonly IHostingEnvironment _env;
		private readonly PopCornDbContext _context;
		private readonly ExportService _exportService;

		public PopCornController(IHostingEnvironment env, PopCornDbContext context, ExportService exportService)
		{
			_env = env;
			_context = context;
			_exportService = exportService;
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
			MemoryCacheHelper.SetMemoryInfo(data);

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
			var news = MemoryCacheHelper.GetNews();
			var postData = news.FirstOrDefault(c => c.Id == data.Id);

			if (postData != null)
			{
				if (data.ScheduleDate.HasValue)
				{
					data.Status = PostStatus.Scheduled;
					if (!postData.ScheduleDate.HasValue)
					{
						SchedulerService.CreateTask(_env.ContentRootPath, data.Id, data.ScheduleDate.Value);
					}
					else if (postData.ScheduleDate != data.ScheduleDate)
					{
						SchedulerService.UpdateTask(_env.ContentRootPath, data.Id, data.ScheduleDate.Value);
					}
				}

				postData = data;
				_context.Update(postData);
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

				news.Add(data);
				_context.Add(data);
				await _context.SaveChangesAsync();

				SchedulerService.CreateTask(_env.ContentRootPath, data.Id, data.ScheduleDate.Value);
			}

			MemoryCacheHelper.SetNews(news);

			return RedirectToAction("Post", "PopCorn", new {id = data.Id});
		}

		public async Task<IActionResult> PublishPost(int postId)
		{
			var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
			if (post != null && post.Status != PostStatus.Published)
			{
				await PopCornBotService.SendNewPostAlert(post);
				if (post.Status == PostStatus.Scheduled)
				{
					SchedulerService.DeleteTask(post.Id);
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

		public string GetTaskInfo(int id)
		{
			try
			{
				var task = SchedulerService.GetTask(id);
				return $"{task.LastTaskResult}, {task.NumberOfMissedRuns}";
			}
			catch (Exception ex)
			{
				return $"{ex.Message}\r\n{ex.StackTrace}";
			}
		}

		public void DeleteTask(int id)
		{
			SchedulerService.DeleteTask(id);
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

