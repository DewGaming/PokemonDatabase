using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedPrecisionToDecimals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Effective",
                table: "TypeCharts",
                type: "decimal(2,1)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Weight",
                table: "Pokemon",
                type: "decimal(4,1)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Height",
                table: "Pokemon",
                type: "decimal(3,1)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Effective",
                table: "TypeCharts",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,1)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Weight",
                table: "Pokemon",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(4,1)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Height",
                table: "Pokemon",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(3,1)");
        }
    }
}
