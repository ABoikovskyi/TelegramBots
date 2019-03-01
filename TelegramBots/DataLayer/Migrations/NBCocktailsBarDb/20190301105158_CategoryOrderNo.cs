using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations.NBCocktailsBarDb
{
    public partial class CategoryOrderNo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderNo",
                table: "IngredientCategories",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderNo",
                table: "IngredientCategories");
        }
    }
}
