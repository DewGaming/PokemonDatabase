using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class RenamedGenderDifferenceBoolForUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowShinyGenderDifferences",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "HideShinyGenderDifferences",
                table: "Users",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HideShinyGenderDifferences",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "ShowShinyGenderDifferences",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
