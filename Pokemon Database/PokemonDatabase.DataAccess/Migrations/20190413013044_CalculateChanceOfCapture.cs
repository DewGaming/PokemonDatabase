using Microsoft.EntityFrameworkCore.Migrations;

namespace PokemonDatabase.DataAccess.Migrations
{
    public partial class CalculateChanceOfCapture : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChanceOfCapture",
                table: "CaptureRates");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ChanceOfCapture",
                table: "CaptureRates",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
