using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class ChangedPokeballCatchModifierToDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "CatchModifier",
                table: "PokeballCatchModifierDetails",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "CatchModifier",
                table: "PokeballCatchModifierDetails",
                type: "real",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
