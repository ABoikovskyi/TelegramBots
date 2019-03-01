using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.NBCocktailsBar
{
	[Table("IngredientCategories")]
	public class IngredientCategory
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int OrderNo { get; set; }
		public int MaxIngredientInCocktail { get; set; }
		public virtual List<Ingredient> Ingredients { get; set; }
	}
}