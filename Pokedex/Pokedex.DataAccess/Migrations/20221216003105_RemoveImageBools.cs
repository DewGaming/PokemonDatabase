using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class RemoveImageBools : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasShinyImage",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "HasThreeDImage",
                table: "Pokemon");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasShinyImage",
                table: "Pokemon",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasThreeDImage",
                table: "Pokemon",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
