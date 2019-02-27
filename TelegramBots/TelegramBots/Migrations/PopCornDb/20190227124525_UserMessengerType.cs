using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramBots.Migrations.PopCornDb
{
    public partial class UserMessengerType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Messenger",
                table: "Users",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Messenger",
                table: "Users");
        }
    }
}
