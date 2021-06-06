using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedGenerationIdToAbilityTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GenerationId",
                table: "Abilities",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_GenerationId",
                table: "Abilities",
                column: "GenerationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Abilities_Generations_GenerationId",
                table: "Abilities",
                column: "GenerationId",
                principalTable: "Generations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Abilities_Generations_GenerationId",
                table: "Abilities");

            migrationBuilder.DropIndex(
                name: "IX_Abilities_GenerationId",
                table: "Abilities");

            migrationBuilder.DropColumn(
                name: "GenerationId",
                table: "Abilities");
        }
    }
}
