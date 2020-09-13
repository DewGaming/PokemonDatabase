using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class RemoveDescriptionAddGameIdForMoves : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Moves");

            migrationBuilder.AlterColumn<int>(
                name: "GenerationId",
                table: "PokemonEggGroupDetails",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "Moves",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Moves_GameId",
                table: "Moves",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Moves_Games_GameId",
                table: "Moves",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Moves_Games_GameId",
                table: "Moves");

            migrationBuilder.DropIndex(
                name: "IX_Moves_GameId",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "Moves");

            migrationBuilder.AlterColumn<int>(
                name: "GenerationId",
                table: "PokemonEggGroupDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Moves",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
