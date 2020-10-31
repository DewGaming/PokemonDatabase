using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedPokemonCaptureRateDetailsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PokemonCaptureRateDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PokemonId = table.Column<int>(nullable: false),
                    CaptureRateId = table.Column<int>(nullable: false),
                    GenerationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonCaptureRateDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PokemonCaptureRateDetails_CaptureRates_CaptureRateId",
                        column: x => x.CaptureRateId,
                        principalTable: "CaptureRates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonCaptureRateDetails_Generations_GenerationId",
                        column: x => x.GenerationId,
                        principalTable: "Generations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonCaptureRateDetails_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PokemonCaptureRateDetails_CaptureRateId",
                table: "PokemonCaptureRateDetails",
                column: "CaptureRateId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonCaptureRateDetails_GenerationId",
                table: "PokemonCaptureRateDetails",
                column: "GenerationId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonCaptureRateDetails_PokemonId",
                table: "PokemonCaptureRateDetails",
                column: "PokemonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PokemonCaptureRateDetails");
        }
    }
}
