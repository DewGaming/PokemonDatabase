using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class RemovedFirstPokemonNecessityFromTeams : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FirstPokemonId",
                table: "PokemonTeams",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FirstPokemonId",
                table: "PokemonTeams",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
