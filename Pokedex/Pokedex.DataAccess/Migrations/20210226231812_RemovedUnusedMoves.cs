using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class RemovedUnusedMoves : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Moves");

            migrationBuilder.DropTable(
                name: "PokemonLocations");

            migrationBuilder.DropTable(
                name: "MoveCategories");

            migrationBuilder.AlterColumn<int>(
                name: "GenerationId",
                table: "Types",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "GenerationId",
                table: "Types",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateTable(
                name: "MoveCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoveCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PokemonLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Condition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    HowToObtain = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaxLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    MinLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    PokemonId = table.Column<int>(type: "int", nullable: false),
                    Rarity = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PokemonLocations_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonLocations_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Moves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Accuracy = table.Column<byte>(type: "tinyint", nullable: true),
                    BasePower = table.Column<byte>(type: "tinyint", nullable: true),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    MoveCategoryId = table.Column<int>(type: "int", nullable: false),
                    MoveTypeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PP = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Moves_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Moves_MoveCategories_MoveCategoryId",
                        column: x => x.MoveCategoryId,
                        principalTable: "MoveCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Moves_Types_MoveTypeId",
                        column: x => x.MoveTypeId,
                        principalTable: "Types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Moves_GameId",
                table: "Moves",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_MoveCategoryId",
                table: "Moves",
                column: "MoveCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_MoveTypeId",
                table: "Moves",
                column: "MoveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLocations_GameId",
                table: "PokemonLocations",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLocations_PokemonId",
                table: "PokemonLocations",
                column: "PokemonId");
        }
    }
}
