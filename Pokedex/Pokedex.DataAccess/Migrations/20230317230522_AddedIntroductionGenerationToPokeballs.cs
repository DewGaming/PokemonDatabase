using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedIntroductionGenerationToPokeballs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GenerationId",
                table: "Pokeballs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pokeballs_GenerationId",
                table: "Pokeballs",
                column: "GenerationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pokeballs_Generations_GenerationId",
                table: "Pokeballs",
                column: "GenerationId",
                principalTable: "Generations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pokeballs_Generations_GenerationId",
                table: "Pokeballs");

            migrationBuilder.DropIndex(
                name: "IX_Pokeballs_GenerationId",
                table: "Pokeballs");

            migrationBuilder.DropColumn(
                name: "GenerationId",
                table: "Pokeballs");
        }
    }
}
