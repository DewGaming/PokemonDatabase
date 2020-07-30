using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedGenerationToTypeDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GenerationId",
                table: "PokemonTypeDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTypeDetails_GenerationId",
                table: "PokemonTypeDetails",
                column: "GenerationId");

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTypeDetails_Generations_GenerationId",
                table: "PokemonTypeDetails",
                column: "GenerationId",
                principalTable: "Generations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTypeDetails_Generations_GenerationId",
                table: "PokemonTypeDetails");

            migrationBuilder.DropIndex(
                name: "IX_PokemonTypeDetails_GenerationId",
                table: "PokemonTypeDetails");

            migrationBuilder.DropColumn(
                name: "GenerationId",
                table: "PokemonTypeDetails");
        }
    }
}
