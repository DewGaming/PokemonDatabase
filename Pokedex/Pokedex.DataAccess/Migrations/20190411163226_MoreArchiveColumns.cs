using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class MoreArchiveColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "GenderRatios",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "ExperienceGrowths",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "EggCycles",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Classifications",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "CaptureRates",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "BaseHappiness",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "GenderRatios");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "ExperienceGrowths");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "EggCycles");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Classifications");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "CaptureRates");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "BaseHappiness");
        }
    }
}
