using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddLegendaryType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "PokemonFormDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "LegendaryTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(nullable: false),
                    IsArchived = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegendaryTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PokemonLegendaryDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LegendaryTypeId = table.Column<int>(nullable: false),
                    PokemonId = table.Column<string>(nullable: false),
                    IsArchived = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonLegendaryDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PokemonLegendaryDetails_LegendaryTypes_LegendaryTypeId",
                        column: x => x.LegendaryTypeId,
                        principalTable: "LegendaryTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonLegendaryDetails_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLegendaryDetails_LegendaryTypeId",
                table: "PokemonLegendaryDetails",
                column: "LegendaryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLegendaryDetails_PokemonId",
                table: "PokemonLegendaryDetails",
                column: "PokemonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PokemonLegendaryDetails");

            migrationBuilder.DropTable(
                name: "LegendaryTypes");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "PokemonFormDetails");
        }
    }
}
