using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedPokemonImageBools : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewedPokemons");

            migrationBuilder.AddColumn<bool>(
                name: "HasShinyImage",
                table: "Pokemon",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasThreeDImage",
                table: "Pokemon",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasShinyImage",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "HasThreeDImage",
                table: "Pokemon");

            migrationBuilder.CreateTable(
                name: "ReviewedPokemons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PokemonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewedPokemons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewedPokemons_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewedPokemons_PokemonId",
                table: "ReviewedPokemons",
                column: "PokemonId");
        }
    }
}
