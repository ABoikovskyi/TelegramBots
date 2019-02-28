using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.NBCocktailsBar
{
	[Table("Ingredients")]
	public class Ingredient
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int CategoryId { get; set; }
		public virtual IngredientCategory Category { get; set; }
		public virtual List<CocktailIngredient> Cocktails { get; set; }
	}
}