using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class ChangedHowPageStatsWork : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastVisit",
                table: "PageStats");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "PageStats");

            migrationBuilder.AddColumn<DateTime>(
                name: "VisitTime",
                table: "PageStats",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VisitTime",
                table: "PageStats");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastVisit",
                table: "PageStats",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "PageStats",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
