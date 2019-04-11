using Microsoft.EntityFrameworkCore.Migrations;

namespace PokemonDatabase.DataAccess.Migrations
{
    public partial class TotalsCalculatedNow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EVTotal",
                table: "EVYields");

            migrationBuilder.DropColumn(
                name: "StatTotal",
                table: "BaseStats");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "EVTotal",
                table: "EVYields",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "StatTotal",
                table: "BaseStats",
                nullable: false,
                defaultValue: (short)0);
        }
    }
}
