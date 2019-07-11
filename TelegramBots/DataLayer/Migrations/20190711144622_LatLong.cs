using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class LatLong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "DrinkHistory");

            migrationBuilder.AddColumn<float>(
                name: "Latitude",
                table: "DrinkHistory",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Longitude",
                table: "DrinkHistory",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "DrinkHistory");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "DrinkHistory");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "DrinkHistory",
                nullable: true);
        }
    }
}
