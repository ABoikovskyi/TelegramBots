using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models.Enums
{
	public enum NumberOfPeople
	{
		[Display(Name = "1")]
		One = 1,

		[Display(Name = "2")]
		Two,
	}
}