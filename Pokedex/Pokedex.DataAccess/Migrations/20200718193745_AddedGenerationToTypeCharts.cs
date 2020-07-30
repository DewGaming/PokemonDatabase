using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedGenerationToTypeCharts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GenerationId",
                table: "TypeCharts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TypeCharts_GenerationId",
                table: "TypeCharts",
                column: "GenerationId");

            migrationBuilder.AddForeignKey(
                name: "FK_TypeCharts_Generations_GenerationId",
                table: "TypeCharts",
                column: "GenerationId",
                principalTable: "Generations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TypeCharts_Generations_GenerationId",
                table: "TypeCharts");

            migrationBuilder.DropIndex(
                name: "IX_TypeCharts_GenerationId",
                table: "TypeCharts");

            migrationBuilder.DropColumn(
                name: "GenerationId",
                table: "TypeCharts");
        }
    }
}
