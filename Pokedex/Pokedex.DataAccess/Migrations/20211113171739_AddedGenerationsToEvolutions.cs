using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedGenerationsToEvolutions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GenerationId",
                table: "Evolutions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_GenerationId",
                table: "Evolutions",
                column: "GenerationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Evolutions_Generations_GenerationId",
                table: "Evolutions",
                column: "GenerationId",
                principalTable: "Generations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evolutions_Generations_GenerationId",
                table: "Evolutions");

            migrationBuilder.DropIndex(
                name: "IX_Evolutions_GenerationId",
                table: "Evolutions");

            migrationBuilder.DropColumn(
                name: "GenerationId",
                table: "Evolutions");
        }
    }
}
