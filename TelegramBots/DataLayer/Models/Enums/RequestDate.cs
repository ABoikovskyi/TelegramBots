using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models.Enums
{
	public enum RequestDate
	{
		[Display(Name= "Сегодня")]
		Today = 1,
		[Display(Name = "Завтра")]
		Tomorrow,
		[Display(Name = "Выберу сам (формат 11.12.2018)")]
		MyChoice
	}
}