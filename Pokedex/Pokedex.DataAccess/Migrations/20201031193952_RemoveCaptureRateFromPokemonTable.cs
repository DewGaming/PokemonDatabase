using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class RemoveCaptureRateFromPokemonTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_CaptureRates_CaptureRateId",
                table: "Pokemon");

            migrationBuilder.DropIndex(
                name: "IX_Pokemon_CaptureRateId",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "CaptureRateId",
                table: "Pokemon");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CaptureRateId",
                table: "Pokemon",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_CaptureRateId",
                table: "Pokemon",
                column: "CaptureRateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_CaptureRates_CaptureRateId",
                table: "Pokemon",
                column: "CaptureRateId",
                principalTable: "CaptureRates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
