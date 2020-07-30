using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedGenerationToEVYields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GenerationId",
                table: "EVYields",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EVYields_GenerationId",
                table: "EVYields",
                column: "GenerationId");

            migrationBuilder.AddForeignKey(
                name: "FK_EVYields_Generations_GenerationId",
                table: "EVYields",
                column: "GenerationId",
                principalTable: "Generations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EVYields_Generations_GenerationId",
                table: "EVYields");

            migrationBuilder.DropIndex(
                name: "IX_EVYields_GenerationId",
                table: "EVYields");

            migrationBuilder.DropColumn(
                name: "GenerationId",
                table: "EVYields");
        }
    }
}
