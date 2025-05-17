using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedVirtualCollectionsToPokemon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PokemonId",
                table: "BaseHappiness",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BaseHappiness_PokemonId",
                table: "BaseHappiness",
                column: "PokemonId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseHappiness_Pokemon_PokemonId",
                table: "BaseHappiness",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaseHappiness_Pokemon_PokemonId",
                table: "BaseHappiness");

            migrationBuilder.DropIndex(
                name: "IX_BaseHappiness_PokemonId",
                table: "BaseHappiness");

            migrationBuilder.DropColumn(
                name: "PokemonId",
                table: "BaseHappiness");
        }
    }
}
