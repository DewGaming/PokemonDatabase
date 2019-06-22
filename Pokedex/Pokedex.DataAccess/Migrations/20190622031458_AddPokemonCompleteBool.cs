using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddPokemonCompleteBool : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsComplete",
                table: "Pokemon",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsComplete",
                table: "Pokemon");
        }
    }
}
