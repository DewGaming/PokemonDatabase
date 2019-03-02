using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PokemonDatabase.Migrations
{
    public partial class AddPokemonTypeDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PokemonTypeDetails",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PokemonID = table.Column<string>(nullable: true),
                    PrimaryTypeID = table.Column<int>(nullable: false),
                    SecondaryTypeID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonTypeDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PokemonTypeDetails_Pokemon_PokemonID",
                        column: x => x.PokemonID,
                        principalTable: "Pokemon",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonTypeDetails_Types_PrimaryTypeID",
                        column: x => x.PrimaryTypeID,
                        principalTable: "Types",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PokemonTypeDetails_Types_SecondaryTypeID",
                        column: x => x.SecondaryTypeID,
                        principalTable: "Types",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTypeDetails_PokemonID",
                table: "PokemonTypeDetails",
                column: "PokemonID");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTypeDetails_PrimaryTypeID",
                table: "PokemonTypeDetails",
                column: "PrimaryTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTypeDetails_SecondaryTypeID",
                table: "PokemonTypeDetails",
                column: "SecondaryTypeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PokemonTypeDetails");
        }
    }
}
