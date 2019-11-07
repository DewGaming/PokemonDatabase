using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddPokemonGameDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PokemonGameDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GenerationId = table.Column<string>(nullable: false),
                    PokemonId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonGameDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PokemonGameDetails_Generations_GenerationId",
                        column: x => x.GenerationId,
                        principalTable: "Generations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonGameDetails_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewedPokemons_PokemonId",
                table: "ReviewedPokemons",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonGameDetails_GenerationId",
                table: "PokemonGameDetails",
                column: "GenerationId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonGameDetails_PokemonId",
                table: "PokemonGameDetails",
                column: "PokemonId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewedPokemons_Pokemon_PokemonId",
                table: "ReviewedPokemons",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewedPokemons_Pokemon_PokemonId",
                table: "ReviewedPokemons");

            migrationBuilder.DropTable(
                name: "PokemonGameDetails");

            migrationBuilder.DropIndex(
                name: "IX_ReviewedPokemons_PokemonId",
                table: "ReviewedPokemons");
        }
    }
}
