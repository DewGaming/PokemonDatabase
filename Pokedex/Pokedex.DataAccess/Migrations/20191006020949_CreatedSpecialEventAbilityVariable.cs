using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class CreatedSpecialEventAbilityVariable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpecialEventAbilityId",
                table: "PokemonAbilityDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PokemonAbilityDetails_SpecialEventAbilityId",
                table: "PokemonAbilityDetails",
                column: "SpecialEventAbilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonAbilityDetails_Abilities_SpecialEventAbilityId",
                table: "PokemonAbilityDetails",
                column: "SpecialEventAbilityId",
                principalTable: "Abilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PokemonAbilityDetails_Abilities_SpecialEventAbilityId",
                table: "PokemonAbilityDetails");

            migrationBuilder.DropIndex(
                name: "IX_PokemonAbilityDetails_SpecialEventAbilityId",
                table: "PokemonAbilityDetails");

            migrationBuilder.DropColumn(
                name: "SpecialEventAbilityId",
                table: "PokemonAbilityDetails");
        }
    }
}
