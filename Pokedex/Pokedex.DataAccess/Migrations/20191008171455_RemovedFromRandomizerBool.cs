using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class RemovedFromRandomizerBool : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromTeamRandomizer",
                table: "PokemonTeams");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FromTeamRandomizer",
                table: "PokemonTeams",
                nullable: false,
                defaultValue: false);
        }
    }
}
