using DataLayer.Models.NBCocktailsBar;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Context
{
	public class NBCocktailsBarDbContext : DbContext
	{
		public NBCocktailsBarDbContext(DbContextOptions<NBCocktailsBarDbContext> options) : base(options)
		{
		}

		public virtual DbSet<IngredientCategory> IngredientCategories { get; set; }
		public virtual DbSet<Ingredient> Ingredients { get; set; }
		public virtual DbSet<Cocktail> Cocktails { get; set; }
		public virtual DbSet<CocktailIngredient> CocktailIngredients { get; set; }
	}
}