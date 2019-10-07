using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddVariableToPokemonTeam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GenerationId",
                table: "PokemonTeams",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PokemonTeamName",
                table: "PokemonTeams",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTeams_GenerationId",
                table: "PokemonTeams",
                column: "GenerationId");

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeams_Generations_GenerationId",
                table: "PokemonTeams",
                column: "GenerationId",
                principalTable: "Generations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeams_Generations_GenerationId",
                table: "PokemonTeams");

            migrationBuilder.DropIndex(
                name: "IX_PokemonTeams_GenerationId",
                table: "PokemonTeams");

            migrationBuilder.DropColumn(
                name: "GenerationId",
                table: "PokemonTeams");

            migrationBuilder.DropColumn(
                name: "PokemonTeamName",
                table: "PokemonTeams");
        }
    }
}
