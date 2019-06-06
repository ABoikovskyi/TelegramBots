using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations.FestivalDb
{
    public partial class SubscriptionType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "UserSubscription",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "UserSubscription",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscription_ScheduleId",
                table: "UserSubscription",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscription_Schedule_ScheduleId",
                table: "UserSubscription",
                column: "ScheduleId",
                principalTable: "Schedule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscription_Schedule_ScheduleId",
                table: "UserSubscription");

            migrationBuilder.DropIndex(
                name: "IX_UserSubscription_ScheduleId",
                table: "UserSubscription");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "UserSubscription");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "UserSubscription");
        }
    }
}
