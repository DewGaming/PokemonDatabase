using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddPokeballGameDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PokeballGameDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PokeballId = table.Column<int>(nullable: false),
                    GameId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokeballGameDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PokeballGameDetails_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokeballGameDetails_Pokeballs_PokeballId",
                        column: x => x.PokeballId,
                        principalTable: "Pokeballs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PokeballGameDetails_GameId",
                table: "PokeballGameDetails",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_PokeballGameDetails_PokeballId",
                table: "PokeballGameDetails",
                column: "PokeballId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PokeballGameDetails");
        }
    }
}
