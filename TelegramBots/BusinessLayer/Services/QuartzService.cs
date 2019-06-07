using System;
using System.Net;
using System.Threading.Tasks;
using BusinessLayer.Helpers;
using DataLayer.Context;
using Quartz;
using Quartz.Impl;

namespace BusinessLayer.Services
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

        public static async Task DeletePostPublishJob(int postId)
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();
            await scheduler.DeleteJob(new JobKey($"PublishPost{postId}", "PublishGroup"));
        }

        public static async Task StartFestivalPostPublisherJob(int postId, DateTime dateToStart)
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            var job = JobBuilder.Create<FestivalPostPublisherJob>().UsingJobData("postId", postId).Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"FestivalPublishPost{postId}", "PublishGroup")
                .StartAt(dateToStart)
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        public static async Task DeleteFestivalPostPublishJob(int postId)
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();
            await scheduler.DeleteJob(new JobKey($"FestivalPublishPost{postId}", "PublishGroup"));
        }

        public static async Task StartNotifyUserJob(int notifyId, DateTime dateToStart)
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            var job = JobBuilder.Create<NotifyUserJob>().UsingJobData("notifyId", notifyId).Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"NotifyUser{notifyId}", "PublishGroup")
                .StartAt(dateToStart)
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        public static async Task DeleteNotifyUserJob(int notifyId)
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();
            await scheduler.DeleteJob(new JobKey($"NotifyUser{notifyId}", "PublishGroup"));
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

        public static void ResetFestivalJobs()
        {
            //var context = new FestivalDbContext()
        }
    }

	public class PostPublisherJob : IJob
	{
		public async Task Execute(IJobExecutionContext context)
		{
			var postId = Convert.ToInt32(context.JobDetail.JobDataMap["postId"]);
            var request = WebRequest.Create($"{Links.AppLink}/popcorn/PublishPost?postId={postId}");
			request.Method = "GET";
			using (var response = (HttpWebResponse)await request.GetResponseAsync())
			{
				response.GetResponseStream();
			}
		}
	}
    
    public class FestivalPostPublisherJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var postId = Convert.ToInt32(context.JobDetail.JobDataMap["postId"]);
            var request = WebRequest.Create($"{Links.AppLink}/festival/PublishPost?postId={postId}");
            request.Method = "GET";
            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            {
                response.GetResponseStream();
            }
        }
    }

    public class NotifyUserJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var notifyId = Convert.ToInt32(context.JobDetail.JobDataMap["notifyId"]);
            var request = WebRequest.Create($"{Links.AppLink}/festival/NotifyUser?notifyId={notifyId}");
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
			var request = WebRequest.Create($"{Links.AppLink}/popcorn/posts");
			request.Method = "GET";
			using (var response = (HttpWebResponse)await request.GetResponseAsync())
			{
				response.GetResponseStream();
			}
		}
	}
}