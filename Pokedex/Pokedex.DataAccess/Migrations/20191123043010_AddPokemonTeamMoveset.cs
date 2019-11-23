using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddPokemonTeamMoveset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PokemonTeamMovesetId",
                table: "PokemonTeamDetails",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PokemonTeamMovesets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstMove = table.Column<string>(nullable: true),
                    SecondMove = table.Column<string>(nullable: true),
                    ThirdMove = table.Column<string>(nullable: true),
                    FourthMove = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonTeamMovesets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTeamDetails_PokemonTeamMovesetId",
                table: "PokemonTeamDetails",
                column: "PokemonTeamMovesetId");

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeamDetails_PokemonTeamMovesets_PokemonTeamMovesetId",
                table: "PokemonTeamDetails",
                column: "PokemonTeamMovesetId",
                principalTable: "PokemonTeamMovesets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeamDetails_PokemonTeamMovesets_PokemonTeamMovesetId",
                table: "PokemonTeamDetails");

            migrationBuilder.DropTable(
                name: "PokemonTeamMovesets");

            migrationBuilder.DropIndex(
                name: "IX_PokemonTeamDetails_PokemonTeamMovesetId",
                table: "PokemonTeamDetails");

            migrationBuilder.DropColumn(
                name: "PokemonTeamMovesetId",
                table: "PokemonTeamDetails");
        }
    }
}
