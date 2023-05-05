using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedPhaseAndTotalEncounters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentPhaseEncounters",
                table: "ShinyHunts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalEncounters",
                table: "ShinyHunts",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentPhaseEncounters",
                table: "ShinyHunts");

            migrationBuilder.DropColumn(
                name: "TotalEncounters",
                table: "ShinyHunts");
        }
    }
}
