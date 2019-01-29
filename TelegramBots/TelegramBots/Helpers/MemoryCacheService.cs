using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using DataLayer.Models;
using Microsoft.Extensions.DependencyInjection;
using TelegramBots.Context;

namespace TelegramBots.Helpers
{
    public static class MemoryCacheHelper
    {
        public static IServiceProvider ServiceProvider;
        private static readonly object MemoryObj = new object();
        private static readonly MemoryCache Memory = MemoryCache.Default;
        private const string MainInfoKey = "MainInfo";
        private const string ConcertsKey = "Concerts";
	    private const string NewsKey = "News";
	    private const int CacheTimeout = 60;

	    public static MainInfo GetMainInfo()
        {
            var mainInfo = MemoryGet<MainInfo>(MainInfoKey);
            if(mainInfo == null)
            {
                using (var serviceScope = ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var dbContext = serviceScope.ServiceProvider.GetService<PopCornDbContext>();
                    mainInfo = dbContext.MainInfo.First();
                }

                SetMemoryInfo(mainInfo);
            }

            return mainInfo;
        }

        public static void SetMemoryInfo(MainInfo mainInfo)
        {
            MemoryRemove(MainInfoKey);
            MemorySet(MainInfoKey, mainInfo);
        }

        public static List<Concert> GetConcerts()
        {
            var concerts = MemoryGet<List<Concert>>(ConcertsKey);
            if (concerts == null)
            {
                using (var serviceScope = ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var dbContext = serviceScope.ServiceProvider.GetService<PopCornDbContext>();
                    concerts = dbContext.Concerts.ToList();
                }

                SetConcerts(concerts);
            }

            return concerts;
        }

        public static void SetConcerts(List<Concert> concerts)
        {
            MemoryRemove(ConcertsKey);
            MemorySet(ConcertsKey, concerts);
        }

	    public static List<News> GetNews()
	    {
		    var news = MemoryGet<List<News>>(NewsKey);
		    if (news == null)
		    {
			    using (var serviceScope = ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
			    {
				    var dbContext = serviceScope.ServiceProvider.GetService<PopCornDbContext>();
				    news = dbContext.News.ToList();
			    }

			    SetNews(news);
		    }

		    return news;
	    }

	    public static void SetNews(List<News> news)
	    {
		    MemoryRemove(NewsKey);
		    MemorySet(NewsKey, news);
	    }

	    private static void MemorySet<T>(string key, T model, int expMin = CacheTimeout)
        {
            lock (MemoryObj)
            {
                Memory.Add(key, model, DateTime.UtcNow.AddMinutes(expMin));
            }
        }

        private static T MemoryGet<T>(string key)
        {
            T model;

            lock (MemoryObj)
            {
                model = (T)Memory.Get(key);
            }

            return model;
        }

        private static void MemoryRemove(string key)
        {
            lock (MemoryObj)
            {
                Memory.Remove(key);
            }
        }
    }
}