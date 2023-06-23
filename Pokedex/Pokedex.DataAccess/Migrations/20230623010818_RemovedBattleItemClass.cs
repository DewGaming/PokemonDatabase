using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class RemovedBattleItemClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeamDetails_BattleItems_BattleItemId",
                table: "PokemonTeamDetails");

            migrationBuilder.DropTable(
                name: "BattleItems");

            migrationBuilder.DropIndex(
                name: "IX_PokemonTeamDetails_BattleItemId",
                table: "PokemonTeamDetails");

            migrationBuilder.DropColumn(
                name: "BattleItemId",
                table: "PokemonTeamDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BattleItemId",
                table: "PokemonTeamDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BattleItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GenerationId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OnlyInThisGeneration = table.Column<bool>(type: "bit", nullable: false),
                    PokemonId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BattleItems_Generations_GenerationId",
                        column: x => x.GenerationId,
                        principalTable: "Generations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BattleItems_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTeamDetails_BattleItemId",
                table: "PokemonTeamDetails",
                column: "BattleItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BattleItems_GenerationId",
                table: "BattleItems",
                column: "GenerationId");

            migrationBuilder.CreateIndex(
                name: "IX_BattleItems_PokemonId",
                table: "BattleItems",
                column: "PokemonId");

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeamDetails_BattleItems_BattleItemId",
                table: "PokemonTeamDetails",
                column: "BattleItemId",
                principalTable: "BattleItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
