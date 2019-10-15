using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedSpecificPokemonToBattleItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PokemonId",
                table: "BattleItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BattleItems_PokemonId",
                table: "BattleItems",
                column: "PokemonId");

            migrationBuilder.AddForeignKey(
                name: "FK_BattleItems_Pokemon_PokemonId",
                table: "BattleItems",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BattleItems_Pokemon_PokemonId",
                table: "BattleItems");

            migrationBuilder.DropIndex(
                name: "IX_BattleItems_PokemonId",
                table: "BattleItems");

            migrationBuilder.DropColumn(
                name: "PokemonId",
                table: "BattleItems");
        }
    }
}
