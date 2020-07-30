using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedGenerationToBaseStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GenerationId",
                table: "BaseStats",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BaseStats_GenerationId",
                table: "BaseStats",
                column: "GenerationId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseStats_Generations_GenerationId",
                table: "BaseStats",
                column: "GenerationId",
                principalTable: "Generations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaseStats_Generations_GenerationId",
                table: "BaseStats");

            migrationBuilder.DropIndex(
                name: "IX_BaseStats_GenerationId",
                table: "BaseStats");

            migrationBuilder.DropColumn(
                name: "GenerationId",
                table: "BaseStats");
        }
    }
}
