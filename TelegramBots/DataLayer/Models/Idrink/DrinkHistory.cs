using System;

namespace DataLayer.Models.Idrink
{
	public class DrinkHistory
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public User User { get; set; }
		public DateTime DrinkTime { get; set; }
		public float? Latitude { get; set; }
		public float? Longitude { get; set; }
		public string Beverage { get; set; }
		public string AdditionalInfo { get; set; }
	}
}