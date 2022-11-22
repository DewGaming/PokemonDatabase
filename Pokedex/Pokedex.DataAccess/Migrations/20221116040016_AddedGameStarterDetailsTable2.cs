using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedGameStarterDetailsTable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameStarterDetail_Games_GameId",
                table: "GameStarterDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_GameStarterDetail_Pokemon_PokemonId",
                table: "GameStarterDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameStarterDetail",
                table: "GameStarterDetail");

            migrationBuilder.RenameTable(
                name: "GameStarterDetail",
                newName: "GameStarterDetails");

            migrationBuilder.RenameIndex(
                name: "IX_GameStarterDetail_PokemonId",
                table: "GameStarterDetails",
                newName: "IX_GameStarterDetails_PokemonId");

            migrationBuilder.RenameIndex(
                name: "IX_GameStarterDetail_GameId",
                table: "GameStarterDetails",
                newName: "IX_GameStarterDetails_GameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameStarterDetails",
                table: "GameStarterDetails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameStarterDetails_Games_GameId",
                table: "GameStarterDetails",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GameStarterDetails_Pokemon_PokemonId",
                table: "GameStarterDetails",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameStarterDetails_Games_GameId",
                table: "GameStarterDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_GameStarterDetails_Pokemon_PokemonId",
                table: "GameStarterDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameStarterDetails",
                table: "GameStarterDetails");

            migrationBuilder.RenameTable(
                name: "GameStarterDetails",
                newName: "GameStarterDetail");

            migrationBuilder.RenameIndex(
                name: "IX_GameStarterDetails_PokemonId",
                table: "GameStarterDetail",
                newName: "IX_GameStarterDetail_PokemonId");

            migrationBuilder.RenameIndex(
                name: "IX_GameStarterDetails_GameId",
                table: "GameStarterDetail",
                newName: "IX_GameStarterDetail_GameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameStarterDetail",
                table: "GameStarterDetail",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameStarterDetail_Games_GameId",
                table: "GameStarterDetail",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GameStarterDetail_Pokemon_PokemonId",
                table: "GameStarterDetail",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
