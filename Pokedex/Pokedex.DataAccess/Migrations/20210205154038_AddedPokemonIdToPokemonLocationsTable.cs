using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedPokemonIdToPokemonLocationsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PokemonId",
                table: "PokemonLocations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLocations_PokemonId",
                table: "PokemonLocations",
                column: "PokemonId");

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonLocations_Pokemon_PokemonId",
                table: "PokemonLocations",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PokemonLocations_Pokemon_PokemonId",
                table: "PokemonLocations");

            migrationBuilder.DropIndex(
                name: "IX_PokemonLocations_PokemonId",
                table: "PokemonLocations");

            migrationBuilder.DropColumn(
                name: "PokemonId",
                table: "PokemonLocations");
        }
    }
}
