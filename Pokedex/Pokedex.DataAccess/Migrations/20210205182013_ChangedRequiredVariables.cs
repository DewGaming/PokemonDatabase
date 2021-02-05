using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class ChangedRequiredVariables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "PokemonLocations");

            migrationBuilder.AddColumn<string>(
                name: "Condition",
                table: "PokemonLocations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HowToObtain",
                table: "PokemonLocations",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Condition",
                table: "PokemonLocations");

            migrationBuilder.DropColumn(
                name: "HowToObtain",
                table: "PokemonLocations");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "PokemonLocations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
