using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedGenerationToEggGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GenerationId",
                table: "PokemonEggGroupDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PokemonEggGroupDetails_GenerationId",
                table: "PokemonEggGroupDetails",
                column: "GenerationId");

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonEggGroupDetails_Generations_GenerationId",
                table: "PokemonEggGroupDetails",
                column: "GenerationId",
                principalTable: "Generations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PokemonEggGroupDetails_Generations_GenerationId",
                table: "PokemonEggGroupDetails");

            migrationBuilder.DropIndex(
                name: "IX_PokemonEggGroupDetails_GenerationId",
                table: "PokemonEggGroupDetails");

            migrationBuilder.DropColumn(
                name: "GenerationId",
                table: "PokemonEggGroupDetails");
        }
    }
}
