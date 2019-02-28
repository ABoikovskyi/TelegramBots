using System.Collections.Generic;
using DataLayer.Models.NBCocktailsBar;

namespace DataLayer.Models.DTO
{
	public class MixologyDataCollection
	{
		public virtual List<Ingredient> Ingredients { get; set; }
		public virtual List<Cocktail> Cocktails { get; set; }
	}
}