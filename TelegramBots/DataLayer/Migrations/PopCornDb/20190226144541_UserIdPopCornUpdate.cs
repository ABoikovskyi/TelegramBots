using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations.PopCornDb
{
    public partial class UserIdPopCornUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "ChatId",
                table: "Users",
                nullable: true,
                oldClrType: typeof(long));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ChatId",
                table: "Users",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Users",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
