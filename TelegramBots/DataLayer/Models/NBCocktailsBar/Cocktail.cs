using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.NBCocktailsBar
{
	[Table("Cocktails")]
	public class Cocktail
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public virtual List<CocktailIngredient> Ingredients { get; set; }
	}
}