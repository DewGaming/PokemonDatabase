using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class RemovedPokemonLocations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PokemonLocationGameDetails");

            migrationBuilder.DropTable(
                name: "PokemonLocationSeasonDetails");

            migrationBuilder.DropTable(
                name: "PokemonLocationTimeDetails");

            migrationBuilder.DropTable(
                name: "PokemonLocationWeatherDetails");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropTable(
                name: "Times");

            migrationBuilder.DropTable(
                name: "PokemonLocationDetails");

            migrationBuilder.DropTable(
                name: "Weathers");

            migrationBuilder.DropTable(
                name: "Locations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Times",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Times", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Weathers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weathers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PokemonLocationDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaptureMethodId = table.Column<int>(type: "int", nullable: false),
                    ChanceOfEncounter = table.Column<double>(type: "float", nullable: false),
                    FailedSnag = table.Column<bool>(type: "bit", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    MaximumLevel = table.Column<int>(type: "int", nullable: false),
                    MinimumLevel = table.Column<int>(type: "int", nullable: false),
                    PokemonId = table.Column<int>(type: "int", nullable: false),
                    SOSBattleOnly = table.Column<bool>(type: "bit", nullable: false),
                    SpecialSpawn = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonLocationDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PokemonLocationDetails_CaptureMethods_CaptureMethodId",
                        column: x => x.CaptureMethodId,
                        principalTable: "CaptureMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonLocationDetails_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonLocationDetails_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PokemonLocationGameDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    PokemonLocationDetailId = table.Column<int>(type: "int", nullable: false)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PokemonLocationDetailId = table.Column<int>(type: "int", nullable: false),
                    SeasonId = table.Column<int>(type: "int", nullable: false)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PokemonLocationDetailId = table.Column<int>(type: "int", nullable: false),
                    TimeId = table.Column<int>(type: "int", nullable: false)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PokemonLocationDetailId = table.Column<int>(type: "int", nullable: false),
                    WeatherId = table.Column<int>(type: "int", nullable: false)
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
                name: "IX_Locations_RegionId",
                table: "Locations",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLocationDetails_CaptureMethodId",
                table: "PokemonLocationDetails",
                column: "CaptureMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLocationDetails_LocationId",
                table: "PokemonLocationDetails",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLocationDetails_PokemonId",
                table: "PokemonLocationDetails",
                column: "PokemonId");

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
    }
}
