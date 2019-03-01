using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations.NBCocktailsBarDb
{
    public partial class MaxIngredientInCocktail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxIngredientInCocktail",
                table: "IngredientCategories",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxIngredientInCocktail",
                table: "IngredientCategories");
        }
    }
}
