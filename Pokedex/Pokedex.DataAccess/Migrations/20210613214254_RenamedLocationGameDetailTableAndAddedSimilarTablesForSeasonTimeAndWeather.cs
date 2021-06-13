using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class RenamedLocationGameDetailTableAndAddedSimilarTablesForSeasonTimeAndWeather : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PokemonLocationDetails_Seasons_SeasonId",
                table: "PokemonLocationDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonLocationDetails_Times_TimeId",
                table: "PokemonLocationDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonLocationDetails_Weathers_WeatherId",
                table: "PokemonLocationDetails");

            migrationBuilder.DropTable(
                name: "LocationGameDetails");

            migrationBuilder.DropIndex(
                name: "IX_PokemonLocationDetails_SeasonId",
                table: "PokemonLocationDetails");

            migrationBuilder.DropIndex(
                name: "IX_PokemonLocationDetails_TimeId",
                table: "PokemonLocationDetails");

            migrationBuilder.DropIndex(
                name: "IX_PokemonLocationDetails_WeatherId",
                table: "PokemonLocationDetails");

            migrationBuilder.DropColumn(
                name: "SeasonId",
                table: "PokemonLocationDetails");

            migrationBuilder.DropColumn(
                name: "TimeId",
                table: "PokemonLocationDetails");

            migrationBuilder.DropColumn(
                name: "WeatherId",
                table: "PokemonLocationDetails");

            migrationBuilder.CreateTable(
                name: "PokemonLocationGameDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PokemonLocationDetailId = table.Column<int>(nullable: false),
                    GameId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonLocationGameDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PokemonLocationGameDetails_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonLocationGameDetails_PokemonLocationDetails_PokemonLocationDetailId",
                        column: x => x.PokemonLocationDetailId,
                        principalTable: "PokemonLocationDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PokemonLocationSeasonDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PokemonLocationDetailId = table.Column<int>(nullable: false),
                    SeasonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonLocationSeasonDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PokemonLocationSeasonDetails_PokemonLocationDetails_PokemonLocationDetailId",
                        column: x => x.PokemonLocationDetailId,
                        principalTable: "PokemonLocationDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonLocationSeasonDetails_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PokemonLocationTimeDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PokemonLocationDetailId = table.Column<int>(nullable: false),
                    TimeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonLocationTimeDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PokemonLocationTimeDetails_PokemonLocationDetails_PokemonLocationDetailId",
                        column: x => x.PokemonLocationDetailId,
                        principalTable: "PokemonLocationDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonLocationTimeDetails_Times_TimeId",
                        column: x => x.TimeId,
                        principalTable: "Times",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PokemonLocationWeatherDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PokemonLocationDetailId = table.Column<int>(nullable: false),
                    WeatherId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonLocationWeatherDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PokemonLocationWeatherDetails_PokemonLocationDetails_PokemonLocationDetailId",
                        column: x => x.PokemonLocationDetailId,
                        principalTable: "PokemonLocationDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonLocationWeatherDetails_Weathers_WeatherId",
                        column: x => x.WeatherId,
                        principalTable: "Weathers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLocationGameDetails_GameId",
                table: "PokemonLocationGameDetails",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLocationGameDetails_PokemonLocationDetailId",
                table: "PokemonLocationGameDetails",
                column: "PokemonLocationDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLocationSeasonDetails_PokemonLocationDetailId",
                table: "PokemonLocationSeasonDetails",
                column: "PokemonLocationDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLocationSeasonDetails_SeasonId",
                table: "PokemonLocationSeasonDetails",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLocationTimeDetails_PokemonLocationDetailId",
                table: "PokemonLocationTimeDetails",
                column: "PokemonLocationDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLocationTimeDetails_TimeId",
                table: "PokemonLocationTimeDetails",
                column: "TimeId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLocationWeatherDetails_PokemonLocationDetailId",
                table: "PokemonLocationWeatherDetails",
                column: "PokemonLocationDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLocationWeatherDetails_WeatherId",
                table: "PokemonLocationWeatherDetails",
                column: "WeatherId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PokemonLocationGameDetails");

            migrationBuilder.DropTable(
                name: "PokemonLocationSeasonDetails");

            migrationBuilder.DropTable(
                name: "PokemonLocationTimeDetails");

            migrationBuilder.DropTable(
                name: "PokemonLocationWeatherDetails");

            migrationBuilder.AddColumn<int>(
                name: "SeasonId",
                table: "PokemonLocationDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimeId",
                table: "PokemonLocationDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WeatherId",
                table: "PokemonLocationDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LocationGameDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    PokemonLocationDetailId = table.Column<int>(type: "int", nullable: false)
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
                name: "IX_PokemonLocationDetails_SeasonId",
                table: "PokemonLocationDetails",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLocationDetails_TimeId",
                table: "PokemonLocationDetails",
                column: "TimeId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLocationDetails_WeatherId",
                table: "PokemonLocationDetails",
                column: "WeatherId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationGameDetails_GameId",
                table: "LocationGameDetails",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationGameDetails_PokemonLocationDetailId",
                table: "LocationGameDetails",
                column: "PokemonLocationDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonLocationDetails_Seasons_SeasonId",
                table: "PokemonLocationDetails",
                column: "SeasonId",
                principalTable: "Seasons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonLocationDetails_Times_TimeId",
                table: "PokemonLocationDetails",
                column: "TimeId",
                principalTable: "Times",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonLocationDetails_Weathers_WeatherId",
                table: "PokemonLocationDetails",
                column: "WeatherId",
                principalTable: "Weathers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
