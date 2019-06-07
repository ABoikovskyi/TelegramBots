using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations.FestivalDb
{
    public partial class ImageToLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Artists",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Image",
                table: "Artists",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
