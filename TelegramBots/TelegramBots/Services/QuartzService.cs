using System;
using System.Net;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace TelegramBots.Services
{
	public class QuartzService
	{
		public static async Task StartPostPublisherJob(int postId, DateTime dateToStart)
		{
			var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
			await scheduler.Start();

			var job = JobBuilder.Create<PostPublisherJob>().UsingJobData("postId", postId).Build();

			var trigger = TriggerBuilder.Create()
				.WithIdentity($"PublishPost{postId}", "PublishGroup") 
				.StartAt(dateToStart)
				.Build();

			await scheduler.ScheduleJob(job, trigger);
		}

		public static async Task StartSiteWorkJob()
		{
			var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
			await scheduler.Start();

			var job = JobBuilder.Create<SiteWorkJob>().Build();

			var trigger = TriggerBuilder.Create()
				.WithIdentity("SiteWork", "PublishGroup")
				.WithSimpleSchedule(x => x
					.WithIntervalInMinutes(10)
					.RepeatForever())
				.Build();

			await scheduler.ScheduleJob(job, trigger);
		}

		public static async Task DeleteJob(int postId)
		{
			var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
			await scheduler.Start();
			await scheduler.DeleteJob(new JobKey($"PublishPost{postId}", "PublishGroup"));
		}
	}

	public class PostPublisherJob : IJob
	{
		public async Task Execute(IJobExecutionContext context)
		{
			var postId = Convert.ToInt32(context.JobDetail.JobDataMap["postId"]);
			var request = WebRequest.Create(
					$"https://playzone.ua/popcorn/PublishPost?postId={postId}");
			request.Method = "GET";
			using (var response = (HttpWebResponse)await request.GetResponseAsync())
			{
				response.GetResponseStream();
			}
		}
	}

	public class SiteWorkJob : IJob
	{
		public async Task Execute(IJobExecutionContext context)
		{
			var request = WebRequest.Create("https://playzone.ua/popcorn/posts");
			request.Method = "GET";
			using (var response = (HttpWebResponse)await request.GetResponseAsync())
			{
				response.GetResponseStream();
			}
		}
	}
}