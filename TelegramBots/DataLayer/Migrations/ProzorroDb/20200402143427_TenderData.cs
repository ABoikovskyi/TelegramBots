using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations.ProzorroDb
{
    public partial class TenderData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Amount",
                table: "Tenders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Tenders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Tenders");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Tenders");
        }
    }
}
