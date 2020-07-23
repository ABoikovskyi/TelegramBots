using System;
using System.Runtime.Caching;

namespace BusinessLayer.Helpers
{
	public static class MemoryCacheHelper
	{
		private static readonly object MemoryObj = new object();
		private static readonly MemoryCache Memory = MemoryCache.Default;

		public static T MemoryGet<T>(string key)
		{
			T model;

			lock (MemoryObj)
			{
				model = (T)Memory.Get(key);
			}

			return model;
		}

		public static void MemorySet<T>(string key, T model, int expMin)
		{
			lock (MemoryObj)
			{
				Memory.Remove(key);
				Memory.Add(key, model, DateTime.UtcNow.AddMinutes(expMin));
			}
		}

		public static void MemoryRemove(string key)
		{
			lock (MemoryObj)
			{
				Memory.Remove(key);
			}
		}
	}
}