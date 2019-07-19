using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class Log2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Log_Users_UserId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_UserId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Log");

            migrationBuilder.AddColumn<long>(
                name: "ChatId",
                table: "Log",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "Log");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Log",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Log_UserId",
                table: "Log",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Log_Users_UserId",
                table: "Log",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
