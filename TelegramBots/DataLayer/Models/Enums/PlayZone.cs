using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models.Enums
{
	public enum PlayZone
	{
		[Display(Name = "Play Zone | ТРЦ Караван, 2 этаж")]
		Caravan = -280178097,

		[Display(Name = "Play Zone | ТРК Украина, 3 этаж")]
		Ukraine = -255772676,

		[Display(Name = "Play Zone | ТРЦ Французкий бульвар, 3 этаж")]
		FranceBoulevard = -286689411
	}
}