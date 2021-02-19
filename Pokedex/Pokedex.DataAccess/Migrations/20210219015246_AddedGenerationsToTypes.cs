using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedGenerationsToTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GenerationId",
                table: "Types",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Types_GenerationId",
                table: "Types",
                column: "GenerationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Types_Generations_GenerationId",
                table: "Types",
                column: "GenerationId",
                principalTable: "Generations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Types_Generations_GenerationId",
                table: "Types");

            migrationBuilder.DropIndex(
                name: "IX_Types_GenerationId",
                table: "Types");

            migrationBuilder.DropColumn(
                name: "GenerationId",
                table: "Types");
        }
    }
}
