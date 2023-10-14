using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedRegionalDex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegionalDexes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 25, nullable: false),
                    GameId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionalDexes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegionalDexes_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegionalDexEntries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegionalPokedexNumber = table.Column<int>(nullable: false),
                    RegionalDexId = table.Column<int>(nullable: false),
                    PokemonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionalDexEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegionalDexEntries_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegionalDexEntries_RegionalDexes_RegionalDexId",
                        column: x => x.RegionalDexId,
                        principalTable: "RegionalDexes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegionalDexEntries_PokemonId",
                table: "RegionalDexEntries",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_RegionalDexEntries_RegionalDexId",
                table: "RegionalDexEntries",
                column: "RegionalDexId");

            migrationBuilder.CreateIndex(
                name: "IX_RegionalDexes_GameId",
                table: "RegionalDexes",
                column: "GameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegionalDexEntries");

            migrationBuilder.DropTable(
                name: "RegionalDexes");
        }
    }
}
