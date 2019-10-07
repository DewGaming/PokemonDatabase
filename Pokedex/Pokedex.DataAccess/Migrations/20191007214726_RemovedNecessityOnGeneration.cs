using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class RemovedNecessityOnGeneration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GenerationId",
                table: "PokemonTeams",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GenerationId",
                table: "PokemonTeams",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
