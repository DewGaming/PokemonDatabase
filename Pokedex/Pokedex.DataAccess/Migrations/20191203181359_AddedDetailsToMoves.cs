using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedDetailsToMoves : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Moves",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Moves",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 300);

            migrationBuilder.AddColumn<byte>(
                name: "Accuracy",
                table: "Moves",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "BasePower",
                table: "Moves",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "MoveTypeId",
                table: "Moves",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "PP",
                table: "Moves",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_Moves_MoveTypeId",
                table: "Moves",
                column: "MoveTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Moves_Types_MoveTypeId",
                table: "Moves",
                column: "MoveTypeId",
                principalTable: "Types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Moves_Types_MoveTypeId",
                table: "Moves");

            migrationBuilder.DropIndex(
                name: "IX_Moves_MoveTypeId",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "Accuracy",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "BasePower",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "MoveTypeId",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "PP",
                table: "Moves");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Moves",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Moves",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
