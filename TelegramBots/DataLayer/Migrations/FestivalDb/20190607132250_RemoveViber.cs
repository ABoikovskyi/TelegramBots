using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations.FestivalDb
{
    public partial class RemoveViber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ChatId",
                table: "Users",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ChatId",
                table: "Users",
                nullable: true,
                oldClrType: typeof(long));
        }
    }
}
