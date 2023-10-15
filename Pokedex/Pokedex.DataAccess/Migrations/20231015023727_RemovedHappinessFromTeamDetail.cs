using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class RemovedHappinessFromTeamDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Happiness",
                table: "PokemonTeamDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Happiness",
                table: "PokemonTeamDetails",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
