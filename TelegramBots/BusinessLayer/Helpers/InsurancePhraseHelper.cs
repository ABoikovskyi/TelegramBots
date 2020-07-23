using System.IO;
using DataLayer.Models.DTO;
using Newtonsoft.Json;

namespace BusinessLayer.Helpers
{
	public static class InsurancePhraseHelper
	{
		public static string WebRootPath;

		public static InsurancePhrases GetPhrases()
		{
			const string cacheKey = "insurance_phrases";
			var data = MemoryCacheHelper.MemoryGet<InsurancePhrases>(cacheKey);
			if (data != null)
			{
				return data;
			}

			using var r = new StreamReader(Path.Combine(WebRootPath, "phrases.txt"));
			var json = r.ReadToEnd();
			data = JsonConvert.DeserializeObject<InsurancePhrases>(json);
			MemoryCacheHelper.MemorySet(cacheKey, data, 10);

			return data;
		}
	}
}