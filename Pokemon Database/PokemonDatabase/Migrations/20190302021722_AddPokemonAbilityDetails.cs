using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PokemonDatabase.Migrations
{
    public partial class AddPokemonAbilityDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PokemonAbilityDetails",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PokemonID = table.Column<string>(nullable: true),
                    PrimaryAbilityID = table.Column<int>(nullable: false),
                    SecondaryAbilityID = table.Column<int>(nullable: true),
                    HiddenAbilityID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonAbilityDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PokemonAbilityDetails_Abilities_HiddenAbilityID",
                        column: x => x.HiddenAbilityID,
                        principalTable: "Abilities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonAbilityDetails_Pokemon_PokemonID",
                        column: x => x.PokemonID,
                        principalTable: "Pokemon",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonAbilityDetails_Abilities_PrimaryAbilityID",
                        column: x => x.PrimaryAbilityID,
                        principalTable: "Abilities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PokemonAbilityDetails_Abilities_SecondaryAbilityID",
                        column: x => x.SecondaryAbilityID,
                        principalTable: "Abilities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PokemonAbilityDetails_HiddenAbilityID",
                table: "PokemonAbilityDetails",
                column: "HiddenAbilityID");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonAbilityDetails_PokemonID",
                table: "PokemonAbilityDetails",
                column: "PokemonID");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonAbilityDetails_PrimaryAbilityID",
                table: "PokemonAbilityDetails",
                column: "PrimaryAbilityID");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonAbilityDetails_SecondaryAbilityID",
                table: "PokemonAbilityDetails",
                column: "SecondaryAbilityID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PokemonAbilityDetails");
        }
    }
}
