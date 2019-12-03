using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedMoveCategoryToMoves : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MoveCategoryId",
                table: "Moves",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Moves_MoveCategoryId",
                table: "Moves",
                column: "MoveCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Moves_MoveCategories_MoveCategoryId",
                table: "Moves",
                column: "MoveCategoryId",
                principalTable: "MoveCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Moves_MoveCategories_MoveCategoryId",
                table: "Moves");

            migrationBuilder.DropIndex(
                name: "IX_Moves_MoveCategoryId",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "MoveCategoryId",
                table: "Moves");
        }
    }
}
