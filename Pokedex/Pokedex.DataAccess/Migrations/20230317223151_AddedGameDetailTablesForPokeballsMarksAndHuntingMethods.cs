using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedGameDetailTablesForPokeballsMarksAndHuntingMethods : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HuntingMethodGameDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(nullable: false),
                    HuntingMethodId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HuntingMethodGameDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HuntingMethodGameDetails_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HuntingMethodGameDetails_HuntingMethods_HuntingMethodId",
                        column: x => x.HuntingMethodId,
                        principalTable: "HuntingMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarkGameDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(nullable: false),
                    MarkId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarkGameDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarkGameDetails_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarkGameDetails_Marks_MarkId",
                        column: x => x.MarkId,
                        principalTable: "Marks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PokeballGameDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(nullable: false),
                    PokeballId = table.Column<int>(nullable: false)
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
                name: "IX_HuntingMethodGameDetails_GameId",
                table: "HuntingMethodGameDetails",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_HuntingMethodGameDetails_HuntingMethodId",
                table: "HuntingMethodGameDetails",
                column: "HuntingMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_MarkGameDetails_GameId",
                table: "MarkGameDetails",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_MarkGameDetails_MarkId",
                table: "MarkGameDetails",
                column: "MarkId");

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
                name: "HuntingMethodGameDetails");

            migrationBuilder.DropTable(
                name: "MarkGameDetails");

            migrationBuilder.DropTable(
                name: "PokeballGameDetails");
        }
    }
}
