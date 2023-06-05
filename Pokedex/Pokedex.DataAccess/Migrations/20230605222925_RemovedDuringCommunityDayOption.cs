using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class RemovedDuringCommunityDayOption : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DuringCommunityDay",
                table: "ShinyHunts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DuringCommunityDay",
                table: "ShinyHunts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
