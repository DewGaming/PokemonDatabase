using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedGenerationToBattleItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GenerationId",
                table: "BattleItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BattleItems_GenerationId",
                table: "BattleItems",
                column: "GenerationId");

            migrationBuilder.AddForeignKey(
                name: "FK_BattleItems_Generations_GenerationId",
                table: "BattleItems",
                column: "GenerationId",
                principalTable: "Generations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BattleItems_Generations_GenerationId",
                table: "BattleItems");

            migrationBuilder.DropIndex(
                name: "IX_BattleItems_GenerationId",
                table: "BattleItems");

            migrationBuilder.DropColumn(
                name: "GenerationId",
                table: "BattleItems");
        }
    }
}
