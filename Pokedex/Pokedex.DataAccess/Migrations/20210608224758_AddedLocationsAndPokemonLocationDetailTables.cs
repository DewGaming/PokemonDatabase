using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedLocationsAndPokemonLocationDetailTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    RegionId = table.Column<int>(nullable: false)
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
                name: "PokemonLocationDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChanceOfEncounter = table.Column<double>(nullable: false),
                    PokemonId = table.Column<int>(nullable: false),
                    LocationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonLocationDetails", x => x.Id);
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

            migrationBuilder.CreateIndex(
                name: "IX_Locations_RegionId",
                table: "Locations",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLocationDetails_LocationId",
                table: "PokemonLocationDetails",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLocationDetails_PokemonId",
                table: "PokemonLocationDetails",
                column: "PokemonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PokemonLocationDetails");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
