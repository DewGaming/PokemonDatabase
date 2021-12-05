using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedWeatherTableBack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Weathers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weathers", x => x.Id);
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
                name: "PokemonLocationWeatherDetails");

            migrationBuilder.DropTable(
                name: "Weathers");
        }
    }
}
