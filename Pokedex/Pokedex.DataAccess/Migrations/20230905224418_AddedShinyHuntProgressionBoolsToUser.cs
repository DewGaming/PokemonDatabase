using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedShinyHuntProgressionBoolsToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HideCapturedShinyPokemon",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowShinyAltForms",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowShinyGenderDifferences",
                table: "Users",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HideCapturedShinyPokemon",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ShowShinyAltForms",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ShowShinyGenderDifferences",
                table: "Users");
        }
    }
}
