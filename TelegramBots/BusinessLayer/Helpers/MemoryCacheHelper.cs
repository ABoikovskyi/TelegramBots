using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using DataLayer.Context;
using DataLayer.Models.NBCocktailsBar;
using DataLayer.Models.PopCorn;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Helpers
{
    public class MemoryCacheHelper
    {
		private readonly PopCornDbContext _popCornContext;
	    private readonly NBCocktailsBarDbContext _nbBarContext;
		private static readonly object MemoryObj = new object();
        private static readonly MemoryCache Memory = MemoryCache.Default;
        private const string MainInfoKey = "MainInfo";
        private const string ConcertsKey = "Concerts";
	    private const string PostsKey = "Posts";
	    private const string IngredientsDataKey = "IngredientsData";
		private const int CacheTimeout = 60;

	    public MemoryCacheHelper(PopCornDbContext popCornContext, NBCocktailsBarDbContext nbBarContext)
	    {
		    _popCornContext = popCornContext;
		    _nbBarContext = nbBarContext;
	    }

	    public MainInfo GetMainInfo()
	    {
		    var mainInfo = MemoryGet<MainInfo>(MainInfoKey);
		    if (mainInfo == null)
		    {
			    mainInfo = _popCornContext.MainInfo.First();
				SetMainInfo(mainInfo);
		    }

		    return mainInfo;
	    }

	    public static void SetMainInfo(MainInfo mainInfo)
        {
            MemoryRemove(MainInfoKey);
            MemorySet(MainInfoKey, mainInfo);
        }

	    public List<Concert> GetConcerts()
	    {
		    var concerts = MemoryGet<List<Concert>>(ConcertsKey);
		    if (concerts == null)
		    {
			    concerts = _popCornContext.Concerts.ToList();
				SetConcerts(concerts);
		    }

		    return concerts;
	    }

	    public static void SetConcerts(List<Concert> concerts)
        {
            MemoryRemove(ConcertsKey);
            MemorySet(ConcertsKey, concerts);
        }

	    public List<Post> GetNews()
	    {
		    var news = MemoryGet<List<Post>>(PostsKey);
		    if (news == null)
		    {
			    news = _popCornContext.Posts.ToList();
				SetNews(news);
		    }

		    return news;
	    }

	    public static void SetNews(List<Post> news)
	    {
		    MemoryRemove(PostsKey);
		    MemorySet(PostsKey, news);
		}

	    public Dictionary<IngredientCategory, List<Ingredient>> GetIngredientsData()
	    {
		    var ingredientsData = MemoryGet<Dictionary<IngredientCategory, List<Ingredient>>>(IngredientsDataKey);
		    if (ingredientsData == null)
		    {
			    ingredientsData = _nbBarContext.Ingredients.Include(i => i.Category).GroupBy(i => i.Category)
				    .ToDictionary(g => g.Key, g => g.OrderBy(i => i.Name).ToList());
			    SetIngredientsData(ingredientsData);
		    }

		    return ingredientsData;
		}

	    public static void SetIngredientsData(Dictionary<IngredientCategory, List<Ingredient>> data)
	    {
		    MemoryRemove(IngredientsDataKey);
			MemorySet(IngredientsDataKey, data);
	    }

		public static void RemoveIngredientsData()
	    {
		    MemoryRemove(IngredientsDataKey);
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