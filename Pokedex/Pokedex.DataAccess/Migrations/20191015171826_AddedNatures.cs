using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedNatures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NatureId",
                table: "PokemonTeamDetails",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Natures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Natures", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTeamDetails_NatureId",
                table: "PokemonTeamDetails",
                column: "NatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeamDetails_Natures_NatureId",
                table: "PokemonTeamDetails",
                column: "NatureId",
                principalTable: "Natures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeamDetails_Natures_NatureId",
                table: "PokemonTeamDetails");

            migrationBuilder.DropTable(
                name: "Natures");

            migrationBuilder.DropIndex(
                name: "IX_PokemonTeamDetails_NatureId",
                table: "PokemonTeamDetails");

            migrationBuilder.DropColumn(
                name: "NatureId",
                table: "PokemonTeamDetails");
        }
    }
}
