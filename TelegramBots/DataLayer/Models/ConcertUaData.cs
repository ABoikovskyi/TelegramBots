using System.Collections.Generic;

namespace DataLayer.Models
{
	public class ConcertUaResponse
	{
		public ConcertUaResponseData Response { get; set; }
	}
	public class ConcertUaResponseData
	{
		public ConcertUaMetaData MetaData { get; set; }
	}

	public class ConcertUaMetaData
	{
		public List<FanZoneData> FanZones { get; set; }
		public List<SeatData> Seats { get; set; }
		public List<SectorData> Sectors { get; set; }
		public List<SectorContent> SectorsContent { get; set; }
		public List<TicketType> TicketTypes { get; set; }
	}

	public class FanZoneData
	{
		public int NumAvailable { get; set; }
		public string SectorElId { get; set; }
		public long SectorId { get; set; }
		public long TicketTypeId { get; set; }
		public string Title { get; set; }
		public int ZoneType { get; set; }
	}

	public class SeatData
	{
		public long Id { get; set; }
		public string ExtraInfo { get; set; }
		public int Row { get; set; }
		public int Seat { get; set; }
		public string SeatContainer { get; set; }
		public int SeatStatusId { get; set; }
		public string SectorElId { get; set; }
		public string SectorTitle { get; set; }
		public long TicketTypeId { get; set; }
	}

	public class SectorData
	{
		public long Id { get; set; }
		public string Color { get; set; }
		public string IsNested { get; set; }
		public string SectorElId { get; set; }
		public string Title { get; set; }
	}

	public class SectorContent
	{
		public string CurrencyCode { get; set; }
		public int NumAvailable { get; set; }
		public double Price { get; set; }
		public long SectorId { get; set; }
	}

	public class TicketType
	{
		public long Id { get; set; }
		public int ChildTypeId { get; set; }
		public int ChildTypeMaxLimit { get; set; }
		public int ChildTypeMinLimit { get; set; }
		public string Colo { get; set; }
		public string CurrencyCode { get; set; }
		public int GroupId { get; set; }
		public double Price { get; set; }
		public string TitlePrint { get; set; }
	}
}