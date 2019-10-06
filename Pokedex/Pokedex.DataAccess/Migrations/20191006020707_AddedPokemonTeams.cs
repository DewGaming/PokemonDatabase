using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedPokemonTeams : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PokemonTeams",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    FirstPokemonId = table.Column<string>(nullable: false),
                    SecondPokemonId = table.Column<string>(nullable: true),
                    ThirdPokemonId = table.Column<string>(nullable: true),
                    FourthPokemonId = table.Column<string>(nullable: true),
                    FifthPokemonId = table.Column<string>(nullable: true),
                    SixthPokemonId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonTeams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PokemonTeams_Pokemon_FifthPokemonId",
                        column: x => x.FifthPokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonTeams_Pokemon_FirstPokemonId",
                        column: x => x.FirstPokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonTeams_Pokemon_FourthPokemonId",
                        column: x => x.FourthPokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonTeams_Pokemon_SecondPokemonId",
                        column: x => x.SecondPokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonTeams_Pokemon_SixthPokemonId",
                        column: x => x.SixthPokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonTeams_Pokemon_ThirdPokemonId",
                        column: x => x.ThirdPokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonTeams_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTeams_UserId",
                table: "PokemonTeams",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PokemonTeams");
        }
    }
}
