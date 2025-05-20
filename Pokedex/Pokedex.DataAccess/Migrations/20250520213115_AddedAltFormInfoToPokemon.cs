using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedAltFormInfoToPokemon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaseHappiness_Pokemon_PokemonId",
                table: "BaseHappiness");

            migrationBuilder.DropIndex(
                name: "IX_BaseHappiness_PokemonId",
                table: "BaseHappiness");

            migrationBuilder.DropColumn(
                name: "PokemonId",
                table: "BaseHappiness");

            migrationBuilder.AddColumn<int>(
                name: "FormId",
                table: "Pokemon",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasHomeArtwork",
                table: "Pokemon",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasShinyArtwork",
                table: "Pokemon",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OriginalFormId",
                table: "Pokemon",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_FormId",
                table: "Pokemon",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_OriginalFormId",
                table: "Pokemon",
                column: "OriginalFormId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_Forms_FormId",
                table: "Pokemon",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_Pokemon_OriginalFormId",
                table: "Pokemon",
                column: "OriginalFormId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_Forms_FormId",
                table: "Pokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_Pokemon_OriginalFormId",
                table: "Pokemon");

            migrationBuilder.DropIndex(
                name: "IX_Pokemon_FormId",
                table: "Pokemon");

            migrationBuilder.DropIndex(
                name: "IX_Pokemon_OriginalFormId",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "FormId",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "HasHomeArtwork",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "HasShinyArtwork",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "OriginalFormId",
                table: "Pokemon");

            migrationBuilder.AddColumn<int>(
                name: "PokemonId",
                table: "BaseHappiness",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BaseHappiness_PokemonId",
                table: "BaseHappiness",
                column: "PokemonId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseHappiness_Pokemon_PokemonId",
                table: "BaseHappiness",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
