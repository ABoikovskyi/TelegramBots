using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.NBCocktailsBar
{
	[Table("CocktailIngredients")]
	public class CocktailIngredient
	{
		public int Id { get; set; }
		public int CocktailId { get; set; }
		public virtual Cocktail Cocktail { get; set; }
		public int IngredientId { get; set; }
		public virtual Ingredient Ingredient { get; set; }
	}
}