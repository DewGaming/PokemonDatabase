using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedAdditionalInfoForShinyHunts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfCapture",
                table: "ShinyHunts",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "DuringCommunityDay",
                table: "ShinyHunts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Encounters",
                table: "ShinyHunts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasShinyCharm",
                table: "ShinyHunts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCaptured",
                table: "ShinyHunts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "ShinyHunts",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SparklingPowerLevel",
                table: "ShinyHunts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "UsingLures",
                table: "ShinyHunts",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfCapture",
                table: "ShinyHunts");

            migrationBuilder.DropColumn(
                name: "DuringCommunityDay",
                table: "ShinyHunts");

            migrationBuilder.DropColumn(
                name: "Encounters",
                table: "ShinyHunts");

            migrationBuilder.DropColumn(
                name: "HasShinyCharm",
                table: "ShinyHunts");

            migrationBuilder.DropColumn(
                name: "IsCaptured",
                table: "ShinyHunts");

            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "ShinyHunts");

            migrationBuilder.DropColumn(
                name: "SparklingPowerLevel",
                table: "ShinyHunts");

            migrationBuilder.DropColumn(
                name: "UsingLures",
                table: "ShinyHunts");
        }
    }
}
