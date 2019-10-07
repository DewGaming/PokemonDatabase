using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class RemovedTypeDetailFromTeamDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeamDetails_PokemonTypeDetails_PokemonTypeDetailId",
                table: "PokemonTeamDetails");

            migrationBuilder.DropIndex(
                name: "IX_PokemonTeamDetails_PokemonTypeDetailId",
                table: "PokemonTeamDetails");

            migrationBuilder.DropColumn(
                name: "PokemonTypeDetailId",
                table: "PokemonTeamDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PokemonTypeDetailId",
                table: "PokemonTeamDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTeamDetails_PokemonTypeDetailId",
                table: "PokemonTeamDetails",
                column: "PokemonTypeDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeamDetails_PokemonTypeDetails_PokemonTypeDetailId",
                table: "PokemonTeamDetails",
                column: "PokemonTypeDetailId",
                principalTable: "PokemonTypeDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
