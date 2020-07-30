using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedGenerationToAbilities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GenerationId",
                table: "PokemonAbilityDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PokemonAbilityDetails_GenerationId",
                table: "PokemonAbilityDetails",
                column: "GenerationId");

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonAbilityDetails_Generations_GenerationId",
                table: "PokemonAbilityDetails",
                column: "GenerationId",
                principalTable: "Generations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PokemonAbilityDetails_Generations_GenerationId",
                table: "PokemonAbilityDetails");

            migrationBuilder.DropIndex(
                name: "IX_PokemonAbilityDetails_GenerationId",
                table: "PokemonAbilityDetails");

            migrationBuilder.DropColumn(
                name: "GenerationId",
                table: "PokemonAbilityDetails");
        }
    }
}
