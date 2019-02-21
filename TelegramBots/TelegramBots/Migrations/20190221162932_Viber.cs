using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramBots.Migrations
{
    public partial class Viber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserRequests",
                nullable: true,
                oldClrType: typeof(long));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "UserRequests",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
