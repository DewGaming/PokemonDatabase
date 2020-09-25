using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class MovedPokeballCatchModifierToSeparateClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CatchModifier",
                table: "Pokeballs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "CatchModifier",
                table: "Pokeballs",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
