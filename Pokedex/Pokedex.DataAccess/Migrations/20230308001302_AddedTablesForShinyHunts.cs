using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedTablesForShinyHunts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HuntingMethods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HuntingMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Marks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pokeballs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pokeballs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShinyHunts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    PokemonId = table.Column<int>(nullable: false),
                    GameId = table.Column<int>(nullable: false),
                    HuntingMethodId = table.Column<int>(nullable: false),
                    PokeballId = table.Column<int>(nullable: false),
                    MarkId = table.Column<int>(nullable: true),
                    Gender = table.Column<string>(maxLength: 6, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShinyHunts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShinyHunts_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShinyHunts_HuntingMethods_HuntingMethodId",
                        column: x => x.HuntingMethodId,
                        principalTable: "HuntingMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShinyHunts_Marks_MarkId",
                        column: x => x.MarkId,
                        principalTable: "Marks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShinyHunts_Pokeballs_PokeballId",
                        column: x => x.PokeballId,
                        principalTable: "Pokeballs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShinyHunts_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShinyHunts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShinyHunts_GameId",
                table: "ShinyHunts",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_ShinyHunts_HuntingMethodId",
                table: "ShinyHunts",
                column: "HuntingMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_ShinyHunts_MarkId",
                table: "ShinyHunts",
                column: "MarkId");

            migrationBuilder.CreateIndex(
                name: "IX_ShinyHunts_PokeballId",
                table: "ShinyHunts",
                column: "PokeballId");

            migrationBuilder.CreateIndex(
                name: "IX_ShinyHunts_PokemonId",
                table: "ShinyHunts",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_ShinyHunts_UserId",
                table: "ShinyHunts",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShinyHunts");

            migrationBuilder.DropTable(
                name: "HuntingMethods");

            migrationBuilder.DropTable(
                name: "Marks");

            migrationBuilder.DropTable(
                name: "Pokeballs");
        }
    }
}
