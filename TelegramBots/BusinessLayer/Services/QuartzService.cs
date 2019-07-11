using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BusinessLayer.Helpers;
using DataLayer.Context;
using DataLayer.Models.Enums;
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

            var job = JobBuilder.Create<PostPublisherJob>()
                .WithIdentity($"PublishPost{postId}")
                .UsingJobData("postId", postId).Build();
            var trigger = TriggerBuilder.Create().StartAt(dateToStart).Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        public static async Task DeletePostPublishJob(int postId)
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();
            await scheduler.DeleteJob(new JobKey($"PublishPost{postId}"));
        }

        public static async Task StartFestivalPostPublisherJob(int postId, DateTime dateToStart)
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            await DeleteFestivalPostPublishJob(postId, scheduler);
            var job = JobBuilder.Create<FestivalPostPublisherJob>()
                .WithIdentity($"FestivalPublishPost{postId}")
                .UsingJobData("postId", postId).Build();
            var trigger = TriggerBuilder.Create().StartAt(dateToStart).Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        public static async Task DeleteFestivalPostPublishJob(int postId, IScheduler scheduler = null)
        {
            if (scheduler == null)
            {
                scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                await scheduler.Start();
            }

            await scheduler.DeleteJob(new JobKey($"FestivalPublishPost{postId}"));
        }

        public static async Task StartNotifyUserJob(int notifyId, DateTime dateToStart)
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            await DeleteNotifyUserJob(notifyId, scheduler);
            var job = JobBuilder.Create<NotifyUserJob>()
                .WithIdentity($"NotifyUser{notifyId}")
                .UsingJobData("notifyId", notifyId).Build();
            var trigger = TriggerBuilder.Create().StartAt(dateToStart).Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        public static async Task DeleteNotifyUserJob(int notifyId, IScheduler scheduler = null)
        {
            if (scheduler == null)
            {
                scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                await scheduler.Start();
            }

            await scheduler.DeleteJob(new JobKey($"NotifyUser{notifyId}"));
        }

        public static async Task StartSiteWorkJob()
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            var job = JobBuilder.Create<SiteWorkJob>().WithIdentity("SiteWork").Build();
            var trigger = TriggerBuilder.Create()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(10)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        /*public static void ResetFestivalJobs(FestivalDbContext dbContext)
        {
            var now = DateTime.Now;
            var scheduledPosts = dbContext.Posts
                .Where(p => p.ScheduleDate.HasValue && !p.PublishDate.HasValue && p.ScheduleDate > now)
                .Select(p => new {p.Id, ScheduleDate = p.ScheduleDate.Value}).ToList();
            foreach (var post in scheduledPosts)
            {
                Task.Run(() => StartFestivalPostPublisherJob(post.Id, post.ScheduleDate)).Wait();
            }

            var notifyUserSubscriptions = dbContext.UserSubscription
                .Where(s => s.Type == SubscriptionType.Notify && s.Schedule.StartDate > now)
                .Select(s => new {s.Id, ScheduleDate = s.Schedule.StartDate}).ToList();
            foreach (var notify in notifyUserSubscriptions)
            {
                Task.Run(() => StartNotifyUserJob(notify.Id, notify.ScheduleDate.AddMinutes(-10))).Wait();
            }
        }*/
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
            var request = WebRequest.Create($"{Links.AppLink}/home/index");
            request.Method = "GET";
            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            {
                response.GetResponseStream();
            }
        }
    }
}