using DataLayer.Models.NBCocktailsBar;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Context
{
	public class NBCocktailsBarDbContext : DbContext
	{
		public NBCocktailsBarDbContext(DbContextOptions<NBCocktailsBarDbContext> options) : base(options)
		{
		}

		public virtual DbSet<Flavor> Flavors { get; set; }
		public virtual DbSet<AlcoholDrink> AlcoholDrinks { get; set; }
		public virtual DbSet<ABV> ABVs { get; set; }
	}
}