using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class LegendaryTypeIsNowRemoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PokemonLegendaryDetails");

            migrationBuilder.DropTable(
                name: "LegendaryTypes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LegendaryTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegendaryTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PokemonLegendaryDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LegendaryTypeId = table.Column<int>(type: "int", nullable: false),
                    PokemonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonLegendaryDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PokemonLegendaryDetails_LegendaryTypes_LegendaryTypeId",
                        column: x => x.LegendaryTypeId,
                        principalTable: "LegendaryTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonLegendaryDetails_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLegendaryDetails_LegendaryTypeId",
                table: "PokemonLegendaryDetails",
                column: "LegendaryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLegendaryDetails_PokemonId",
                table: "PokemonLegendaryDetails",
                column: "PokemonId");
        }
    }
}
