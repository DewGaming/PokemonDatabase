using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddGenerationToMarks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GenerationId",
                table: "Marks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Marks_GenerationId",
                table: "Marks",
                column: "GenerationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Marks_Generations_GenerationId",
                table: "Marks",
                column: "GenerationId",
                principalTable: "Generations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Marks_Generations_GenerationId",
                table: "Marks");

            migrationBuilder.DropIndex(
                name: "IX_Marks_GenerationId",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "GenerationId",
                table: "Marks");
        }
    }
}
