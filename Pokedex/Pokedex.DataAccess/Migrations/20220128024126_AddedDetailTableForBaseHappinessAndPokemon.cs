using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedDetailTableForBaseHappinessAndPokemon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PokemonBaseHappinessDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaseHappinessId = table.Column<int>(nullable: false),
                    GenerationId = table.Column<int>(nullable: false),
                    PokemonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonBaseHappinessDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PokemonBaseHappinessDetails_BaseHappiness_BaseHappinessId",
                        column: x => x.BaseHappinessId,
                        principalTable: "BaseHappiness",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonBaseHappinessDetails_Generations_GenerationId",
                        column: x => x.GenerationId,
                        principalTable: "Generations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonBaseHappinessDetails_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PokemonBaseHappinessDetails_BaseHappinessId",
                table: "PokemonBaseHappinessDetails",
                column: "BaseHappinessId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonBaseHappinessDetails_GenerationId",
                table: "PokemonBaseHappinessDetails",
                column: "GenerationId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonBaseHappinessDetails_PokemonId",
                table: "PokemonBaseHappinessDetails",
                column: "PokemonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PokemonBaseHappinessDetails");
        }
    }
}
