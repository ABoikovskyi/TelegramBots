using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models.Enums
{
	public enum RequestTime
	{
		[Display(Name = "12:00")] Hours12 = 1,
		[Display(Name = "14:00")] Hours14,
		[Display(Name = "16:00")] Hours16,
		[Display(Name = "18:00")] Hours18,
		[Display(Name = "19:00")] Hours19,

		[Display(Name = "Свое время (в формате 00:00)")]
		MyChoice
	}
}