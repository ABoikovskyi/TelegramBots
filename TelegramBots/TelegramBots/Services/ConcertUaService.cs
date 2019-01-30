﻿using System;
using System.Collections.Generic;
using DataLayer.Models;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace TelegramBots.Services
{
	public static class ConcertUaService
	{
		private static readonly HttpClient Client = new HttpClient();
		
		public static async Task<string> GetProcessedData(string url)
		{
			ConcertUaMetaData data;
			if (url.Contains("concert.ua"))
			{
				data = await GetResponse(url);
			}
			else
			{
				var links = await GetConcertLinks(url);
				if (links.Count > 1)
				{
					return $"Выберите одну из ссылок или уточните поиск:<br>{string.Join("<br>", links)}";
				}
				else
				{
					data = await GetResponse(links[0]);
				}
			}

			var sb = new StringBuilder();
			if (data.FanZones != null && data.FanZones.Count > 0)
			{
				sb.AppendLine("<h3>Фан-зоны:</h3>");
				foreach (var fanZone in data.FanZones)
				{
					sb.AppendLine(
						$"{fanZone.Title}. Цена: {data.TicketTypes.First(t => t.Id == fanZone.TicketTypeId).Price}. Осталось билетов: {fanZone.NumAvailable}<br/>");
				}

				sb.AppendLine($"<h4>Всего осталось в фан-зонах: {data.FanZones.Sum(sc => sc.NumAvailable)}</h4>");
			}

			if (data.Sectors != null && data.Sectors.Count > 0)
			{
				sb.AppendLine("<h3>Сектора:</h3>");
				foreach (var sector in data.Sectors)
				{
					sb.AppendLine($"<h4>{sector.Title}</h4>");
					foreach (var sectorContent in data.SectorsContent.Where(sc=>sc.SectorId == sector.Id))
					{
						sb.AppendLine($"Цена: {sectorContent.Price}. Осталось билетов: {sectorContent.NumAvailable}<br/>");
					}
				}

				sb.AppendLine($"<h4>Всего осталось в секторах: {data.SectorsContent.Sum(sc => sc.NumAvailable)}</h4>");
			}

			if (data.Seats != null && data.Seats.Count > 0)
			{
				sb.AppendLine("<h3>Сидячие места:</h3>");
				foreach (var seatsSector in data.Seats.GroupBy(s=> s.SectorTitle).OrderBy(s=>s.Key))
				{
					sb.AppendLine($"<h4>{seatsSector.Key}</h4>");
					foreach (var sectorSeats in seatsSector.GroupBy(s => new {s.Row, s.TicketTypeId}))
					{
						sb.AppendLine(
							$"Ряд: {sectorSeats.Key.Row}. Цена: {data.TicketTypes.First(t => t.Id == sectorSeats.Key.TicketTypeId).Price}. Осталось билетов: {sectorSeats.Count()}<br/>");
					}

					sb.AppendLine($"<h4>Всего осталось в секторе: {seatsSector.Count()}</h4><br/>");
				}

				sb.AppendLine($"<h4>Всего осталось сидячих мест: {data.Seats.Count}</h4>");
			}

			return sb.ToString();
		}

		private static async Task<ConcertUaMetaData> GetResponse(string url)
		{
			var response = await Client.GetAsync(url);
			var responseString = await response.Content.ReadAsStringAsync();
			var eventId = responseString.Split("\n")
				.FirstOrDefault(l => l.Contains("window.eventId"))?.Replace("window.eventId = ", "")
				.Replace(";", "").Trim();

			var requestParams = new Dictionary<string, string>
			{
				{ "eventId", eventId },
				{ "rootTierId", "" }
			};

			var content = new FormUrlEncodedContent(requestParams);
			response = await Client.PostAsync("https://concert.ua/api/v3/scheme/metadata", content);
			var concertUaData = JsonConvert
				.DeserializeObject<ConcertUaResponse>(await response.Content.ReadAsStringAsync())
				.Response.MetaData;

			return concertUaData;
		}

		private static async Task<List<string>> GetConcertLinks(string url)
		{
			var response = await Client.GetAsync($"https://concert.ua/ru/search-result?query={HttpUtility.HtmlEncode(url)}");
			var responseString = await response.Content.ReadAsStringAsync();
			var result = new List<string>();
			var lines = responseString.Split("\n").Where(s => s.Contains("<a class=\"event"));
			foreach (var line in lines)
			{
				var index = line.IndexOf("href=\"", StringComparison.CurrentCultureIgnoreCase);
				result.Add("concert.ua" + line.Substring(index).Replace("href=\"", "").Replace("\">", ""));
			}

			return result;
		}
	}
}