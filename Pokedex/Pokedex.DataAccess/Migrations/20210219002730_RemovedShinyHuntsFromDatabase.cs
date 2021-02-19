using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class RemovedShinyHuntsFromDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShinyHunts");

            migrationBuilder.DropTable(
                name: "ShinyHuntingTechniques");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShinyHuntingTechniques",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Technique = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShinyHuntingTechniques", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShinyHunts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    HasShinyCharm = table.Column<bool>(type: "bit", nullable: false),
                    HuntComplete = table.Column<bool>(type: "bit", nullable: false),
                    IsPokemonCaught = table.Column<bool>(type: "bit", nullable: false),
                    PokemonId = table.Column<int>(type: "int", nullable: false),
                    ShinyAttemptCount = table.Column<int>(type: "int", nullable: false),
                    ShinyHuntingTechniqueId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
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
                        name: "FK_ShinyHunts_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShinyHunts_ShinyHuntingTechniques_ShinyHuntingTechniqueId",
                        column: x => x.ShinyHuntingTechniqueId,
                        principalTable: "ShinyHuntingTechniques",
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
                name: "IX_ShinyHunts_PokemonId",
                table: "ShinyHunts",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_ShinyHunts_ShinyHuntingTechniqueId",
                table: "ShinyHunts",
                column: "ShinyHuntingTechniqueId");

            migrationBuilder.CreateIndex(
                name: "IX_ShinyHunts_UserId",
                table: "ShinyHunts",
                column: "UserId");
        }
    }
}
