using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class RemovedPokemonTableBaseHappiness : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_BaseHappiness_BaseHappinessId",
                table: "Pokemon");

            migrationBuilder.DropIndex(
                name: "IX_Pokemon_BaseHappinessId",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "BaseHappinessId",
                table: "Pokemon");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BaseHappinessId",
                table: "Pokemon",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_BaseHappinessId",
                table: "Pokemon",
                column: "BaseHappinessId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_BaseHappiness_BaseHappinessId",
                table: "Pokemon",
                column: "BaseHappinessId",
                principalTable: "BaseHappiness",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
