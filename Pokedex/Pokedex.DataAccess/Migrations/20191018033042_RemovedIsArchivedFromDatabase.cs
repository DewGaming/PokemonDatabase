using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class RemovedIsArchivedFromDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Types");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "ShinyHunts");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "ShinyHuntingTechniques");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "PokemonLegendaryDetails");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "PokemonFormDetails");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "LegendaryTypes");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Generations");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "GenderRatios");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "ExperienceGrowths");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "EggGroups");

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

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Abilities");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Types",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "ShinyHunts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "ShinyHuntingTechniques",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "PokemonLegendaryDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "PokemonFormDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Pokemon",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "LegendaryTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Generations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "GenderRatios",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Forms",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "ExperienceGrowths",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "EggGroups",
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

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Abilities",
                nullable: false,
                defaultValue: false);
        }
    }
}
