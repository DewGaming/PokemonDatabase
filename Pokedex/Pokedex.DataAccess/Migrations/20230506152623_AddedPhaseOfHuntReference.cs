using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedPhaseOfHuntReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PhaseOfHuntId",
                table: "ShinyHunts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShinyHunts_PhaseOfHuntId",
                table: "ShinyHunts",
                column: "PhaseOfHuntId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShinyHunts_ShinyHunts_PhaseOfHuntId",
                table: "ShinyHunts",
                column: "PhaseOfHuntId",
                principalTable: "ShinyHunts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShinyHunts_ShinyHunts_PhaseOfHuntId",
                table: "ShinyHunts");

            migrationBuilder.DropIndex(
                name: "IX_ShinyHunts_PhaseOfHuntId",
                table: "ShinyHunts");

            migrationBuilder.DropColumn(
                name: "PhaseOfHuntId",
                table: "ShinyHunts");
        }
    }
}
