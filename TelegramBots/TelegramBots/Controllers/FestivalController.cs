using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Helpers;
using BusinessLayer.Services;
using BusinessLayer.Services.Festival;
using DataLayer.Context;
using DataLayer.Models.DTO;
using DataLayer.Models.Enums;
using DataLayer.Models.Festival;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TelegramBots.Controllers
{
	public class FestivalController : Controller
	{
		private readonly FestivalDbContext _context;
        private readonly FestivalBotServiceTelegram _telegramBotService;

        public FestivalController(FestivalDbContext context, FestivalBotServiceTelegram telegramBotService)
		{
			_context = context;
            _telegramBotService = telegramBotService;
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
            MemoryCacheHelper.SetFestivalInfo(data);


            return RedirectToAction("Festivals");
		}

		public IActionResult Stages()
        {
            return View(_context.Stages.Include(i => i.Festival)
                .GroupBy(i => i.Festival).OrderBy(i => i.Key.Id)
                .ToDictionary(g => g.Key.Name, g => g.OrderBy(i => i.Name).ToList()));
        }

		public IActionResult Stage(int? id)
		{
			ViewBag.Festivals = _context.Festivals.OrderBy(c => c.Id).ToList();

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
            MemoryCacheHelper.RemoveArtists();

            return RedirectToAction("Artists");
        }


        public IActionResult Schedule()
        {
            return View(_context.Schedule
                .Select(s => new {s.Id, s.StartDate, s.EndDate, Stage = s.Stage.Name, Artist = s.Artist.Name}).ToList()
                .GroupBy(s => new DateTime(s.StartDate.Year, s.StartDate.Month, s.StartDate.Day)).OrderBy(i => i.Key)
                .ToDictionary(g => g.Key,
                    g => g.GroupBy(d => d.Stage).ToDictionary(s => s.Key,
                        s => s.Select(a => new ArtistSchedule
                        {
                            Id = a.Id,
                            Artist = a.Artist,
                            StartTime = a.StartDate.ToString("HH:mm"),
                            EndTime = a.EndDate.ToString("HH:mm")
                        }).OrderBy(a => a.EndTime).ToList())));
        }

        public IActionResult ArtistSchedule(int? id)
        {
            ViewBag.Stages = _context.Stages.OrderBy(c => c.Name).ToList();
            ViewBag.Artists = _context.Artists.OrderBy(c => c.Name).Select(c => new { c.Id, c.Name })
                .ToDictionary(c => c.Id, c => c.Name);

            return View(id.HasValue ? _context.Schedule.First(c => c.Id == id.Value) : new Schedule());
        }

        public async Task<IActionResult> ScheduleSave(Schedule data)
        {
            if (_context.Schedule.Any(c => c.Id == data.Id))
            {
                foreach (var subscription in _context.UserSubscription.Where(s => s.ScheduleId == data.Id)
                    .Select(s => s.Id))
                {
                    await QuartzService.DeleteFestivalPostPublishJob(subscription);
                    await QuartzService.StartNotifyUserJob(subscription, data.StartDate.AddMinutes(-10));
                }

                _context.Update(data);
            }
            else
            {
                _context.Add(data);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Schedule");
        }

        public IActionResult Posts()
        {
            return View(_context.Posts.Include(n => n.Artist).ToList());
        }

        public IActionResult Post(int? id)
        {
            ViewBag.Artists = _context.Artists.Select(c => new { c.Id, c.Name })
                .ToDictionary(c => c.Id, c => c.Name);
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
                        await QuartzService.StartFestivalPostPublisherJob(data.Id, data.ScheduleDate.Value);
                    }
                    else if (postData.ScheduleDate != data.ScheduleDate)
                    {
                        await QuartzService.DeleteFestivalPostPublishJob(data.Id);
                        await QuartzService.StartFestivalPostPublisherJob(data.Id, data.ScheduleDate.Value);
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
                    await QuartzService.StartFestivalPostPublisherJob(data.Id, data.ScheduleDate.Value);
                }
            }

            return RedirectToAction("Post", "PopCorn", new { id = data.Id });
        }

        public async Task<IActionResult> PublishPost(int postId)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
            if (post != null && post.Status != PostStatus.Published)
            {
                await _telegramBotService.SendNewPostAlert(post);
                if (post.Status == PostStatus.Scheduled)
                {
                    await QuartzService.DeleteFestivalPostPublishJob(postId);
                }

                post.Status = PostStatus.Published;
                post.PublishDate = DateTime.Now;
                _context.Update(post);
                _context.SaveChanges();
            }

            return RedirectToAction("Posts");
        }

        public async Task NotifyUser(int notifyId)
    {
            await _telegramBotService.SendNotifyMeAlert(notifyId);
            await QuartzService.DeleteFestivalPostPublishJob(notifyId);
        }
    }
}