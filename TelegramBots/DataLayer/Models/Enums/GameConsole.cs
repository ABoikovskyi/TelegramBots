using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models.Enums
{
	public enum GameConsole
	{
		[Display(Name= "Play Station 4")]
		PlayStation = 1,
		[Display(Name = "Xbox One")]
		XboxOne,
		[Display(Name = "PS VR")]
		PsVr
	}
}