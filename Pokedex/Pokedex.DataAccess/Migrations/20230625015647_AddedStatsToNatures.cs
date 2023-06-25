using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedStatsToNatures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoweredStatId",
                table: "Natures",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RaisedStatId",
                table: "Natures",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Natures_LoweredStatId",
                table: "Natures",
                column: "LoweredStatId");

            migrationBuilder.CreateIndex(
                name: "IX_Natures_RaisedStatId",
                table: "Natures",
                column: "RaisedStatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Natures_Stats_LoweredStatId",
                table: "Natures",
                column: "LoweredStatId",
                principalTable: "Stats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Natures_Stats_RaisedStatId",
                table: "Natures",
                column: "RaisedStatId",
                principalTable: "Stats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Natures_Stats_LoweredStatId",
                table: "Natures");

            migrationBuilder.DropForeignKey(
                name: "FK_Natures_Stats_RaisedStatId",
                table: "Natures");

            migrationBuilder.DropIndex(
                name: "IX_Natures_LoweredStatId",
                table: "Natures");

            migrationBuilder.DropIndex(
                name: "IX_Natures_RaisedStatId",
                table: "Natures");

            migrationBuilder.DropColumn(
                name: "LoweredStatId",
                table: "Natures");

            migrationBuilder.DropColumn(
                name: "RaisedStatId",
                table: "Natures");
        }
    }
}
