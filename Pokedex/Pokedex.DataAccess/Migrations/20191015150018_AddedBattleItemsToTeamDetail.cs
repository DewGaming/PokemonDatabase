using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedBattleItemsToTeamDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BattleItemId",
                table: "PokemonTeamDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTeamDetails_BattleItemId",
                table: "PokemonTeamDetails",
                column: "BattleItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeamDetails_BattleItems_BattleItemId",
                table: "PokemonTeamDetails",
                column: "BattleItemId",
                principalTable: "BattleItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeamDetails_BattleItems_BattleItemId",
                table: "PokemonTeamDetails");

            migrationBuilder.DropIndex(
                name: "IX_PokemonTeamDetails_BattleItemId",
                table: "PokemonTeamDetails");

            migrationBuilder.DropColumn(
                name: "BattleItemId",
                table: "PokemonTeamDetails");
        }
    }
}
