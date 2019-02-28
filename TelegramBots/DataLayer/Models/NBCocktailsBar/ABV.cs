using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.NBCocktailsBar
{
	[Table("ABV")]
	public class ABV
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}
}