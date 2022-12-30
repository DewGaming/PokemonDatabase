using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddTeraType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeraTypeId",
                table: "PokemonTeamDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTeamDetails_TeraTypeId",
                table: "PokemonTeamDetails",
                column: "TeraTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeamDetails_Types_TeraTypeId",
                table: "PokemonTeamDetails",
                column: "TeraTypeId",
                principalTable: "Types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeamDetails_Types_TeraTypeId",
                table: "PokemonTeamDetails");

            migrationBuilder.DropIndex(
                name: "IX_PokemonTeamDetails_TeraTypeId",
                table: "PokemonTeamDetails");

            migrationBuilder.DropColumn(
                name: "TeraTypeId",
                table: "PokemonTeamDetails");
        }
    }
}
