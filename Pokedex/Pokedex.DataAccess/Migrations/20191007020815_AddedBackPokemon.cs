using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedBackPokemon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FifthPokemonId",
                table: "PokemonTeams",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FirstPokemonId",
                table: "PokemonTeams",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FourthPokemonId",
                table: "PokemonTeams",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SecondPokemonId",
                table: "PokemonTeams",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SixthPokemonId",
                table: "PokemonTeams",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThirdPokemonId",
                table: "PokemonTeams",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTeams_FifthPokemonId",
                table: "PokemonTeams",
                column: "FifthPokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTeams_FirstPokemonId",
                table: "PokemonTeams",
                column: "FirstPokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTeams_FourthPokemonId",
                table: "PokemonTeams",
                column: "FourthPokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTeams_SecondPokemonId",
                table: "PokemonTeams",
                column: "SecondPokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTeams_SixthPokemonId",
                table: "PokemonTeams",
                column: "SixthPokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTeams_ThirdPokemonId",
                table: "PokemonTeams",
                column: "ThirdPokemonId");

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_FifthPokemonId",
                table: "PokemonTeams",
                column: "FifthPokemonId",
                principalTable: "PokemonTeamDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_FirstPokemonId",
                table: "PokemonTeams",
                column: "FirstPokemonId",
                principalTable: "PokemonTeamDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_FourthPokemonId",
                table: "PokemonTeams",
                column: "FourthPokemonId",
                principalTable: "PokemonTeamDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_SecondPokemonId",
                table: "PokemonTeams",
                column: "SecondPokemonId",
                principalTable: "PokemonTeamDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_SixthPokemonId",
                table: "PokemonTeams",
                column: "SixthPokemonId",
                principalTable: "PokemonTeamDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_ThirdPokemonId",
                table: "PokemonTeams",
                column: "ThirdPokemonId",
                principalTable: "PokemonTeamDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_FifthPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_FirstPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_FourthPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_SecondPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_SixthPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_ThirdPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropIndex(
                name: "IX_PokemonTeams_FifthPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropIndex(
                name: "IX_PokemonTeams_FirstPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropIndex(
                name: "IX_PokemonTeams_FourthPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropIndex(
                name: "IX_PokemonTeams_SecondPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropIndex(
                name: "IX_PokemonTeams_SixthPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropIndex(
                name: "IX_PokemonTeams_ThirdPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropColumn(
                name: "FifthPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropColumn(
                name: "FirstPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropColumn(
                name: "FourthPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropColumn(
                name: "SecondPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropColumn(
                name: "SixthPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropColumn(
                name: "ThirdPokemonId",
                table: "PokemonTeams");
        }
    }
}
