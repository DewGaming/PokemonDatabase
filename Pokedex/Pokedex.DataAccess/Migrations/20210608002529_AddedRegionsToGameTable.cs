using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedRegionsToGameTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "Games",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_RegionId",
                table: "Games",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Regions_RegionId",
                table: "Games",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Regions_RegionId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_RegionId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "Games");
        }
    }
}
