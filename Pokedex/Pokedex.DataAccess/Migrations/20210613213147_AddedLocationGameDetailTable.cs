using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedLocationGameDetailTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocationGameDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PokemonLocationDetailId = table.Column<int>(nullable: false),
                    GameId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationGameDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationGameDetails_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LocationGameDetails_PokemonLocationDetails_PokemonLocationDetailId",
                        column: x => x.PokemonLocationDetailId,
                        principalTable: "PokemonLocationDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationGameDetails_GameId",
                table: "LocationGameDetails",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationGameDetails_PokemonLocationDetailId",
                table: "LocationGameDetails",
                column: "PokemonLocationDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationGameDetails");
        }
    }
}
