using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedDynamicFormOptionBool : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Catchable",
                table: "Forms");

            migrationBuilder.AddColumn<bool>(
                name: "SeparateRandomize",
                table: "Forms",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeparateRandomize",
                table: "Forms");

            migrationBuilder.AddColumn<bool>(
                name: "Catchable",
                table: "Forms",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
