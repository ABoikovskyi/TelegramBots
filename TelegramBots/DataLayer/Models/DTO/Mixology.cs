using System.Collections.Generic;
using DataLayer.Models.NBCocktailsBar;

namespace DataLayer.Models.DTO
{
	public class Mixology
	{
		public virtual List<Flavor> Flavors { get; set; }
		public virtual List<AlcoholDrink> AlcoholDrinks { get; set; }
		public virtual List<ABV> ABVs { get; set; }
	}
}