using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class RemovedRegionDexTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PokemonDLCRegionalDexDetails");

            migrationBuilder.DropTable(
                name: "PokemonRegionalDexDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PokemonDLCRegionalDexDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    PokemonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonDLCRegionalDexDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PokemonDLCRegionalDexDetails_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonDLCRegionalDexDetails_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PokemonRegionalDexDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    PokemonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonRegionalDexDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PokemonRegionalDexDetails_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonRegionalDexDetails_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PokemonDLCRegionalDexDetails_GameId",
                table: "PokemonDLCRegionalDexDetails",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonDLCRegionalDexDetails_PokemonId",
                table: "PokemonDLCRegionalDexDetails",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonRegionalDexDetails_GameId",
                table: "PokemonRegionalDexDetails",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonRegionalDexDetails_PokemonId",
                table: "PokemonRegionalDexDetails",
                column: "PokemonId");
        }
    }
}
