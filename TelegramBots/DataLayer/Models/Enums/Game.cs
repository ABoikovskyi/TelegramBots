using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models.Enums
{
	public enum Game
	{
		[Display(Name = "FIFA 19")] FIFA = 1,
		[Display(Name = "Mortal Kombat X")] MKX,
		[Display(Name = "GTA V")] GTA,

		[Display(Name = "Что-нибудь на двоих")]
		For2,

		[Display(Name = "Что-нибудь на одного")]
		Fo1,
		[Display(Name = "Свой выбор")] MyChoice
	}
}