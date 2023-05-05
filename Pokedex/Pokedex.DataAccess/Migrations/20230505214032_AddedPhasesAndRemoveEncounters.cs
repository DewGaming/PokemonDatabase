using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedPhasesAndRemoveEncounters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Encounters",
                table: "ShinyHunts");

            migrationBuilder.AddColumn<int>(
                name: "Phases",
                table: "ShinyHunts",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phases",
                table: "ShinyHunts");

            migrationBuilder.AddColumn<int>(
                name: "Encounters",
                table: "ShinyHunts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
