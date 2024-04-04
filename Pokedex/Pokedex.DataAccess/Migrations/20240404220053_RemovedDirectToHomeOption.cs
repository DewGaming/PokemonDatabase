using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class RemovedDirectToHomeOption : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DirectHOMETransfer",
                table: "ShinyHunts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DirectHOMETransfer",
                table: "ShinyHunts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
