using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class MadeAccuracyNotRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "BasePower",
                table: "Moves",
                nullable: true,
                oldClrType: typeof(byte));

            migrationBuilder.AlterColumn<byte>(
                name: "Accuracy",
                table: "Moves",
                nullable: true,
                oldClrType: typeof(byte));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "BasePower",
                table: "Moves",
                nullable: false,
                oldClrType: typeof(byte),
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "Accuracy",
                table: "Moves",
                nullable: false,
                oldClrType: typeof(byte),
                oldNullable: true);
        }
    }
}
