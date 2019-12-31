using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class ReaddedPokemonAndGenerationWithIntPrimaryKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Generations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Region = table.Column<string>(maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Generations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pokemon",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PokedexNumber = table.Column<int>(maxLength: 6, nullable: false),
                    Name = table.Column<string>(maxLength: 25, nullable: false),
                    Height = table.Column<decimal>(type: "decimal(3,1)", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(4,1)", nullable: false),
                    ExpYield = table.Column<int>(nullable: false),
                    IsComplete = table.Column<bool>(nullable: false),
                    EggCycleId = table.Column<int>(nullable: false),
                    GenderRatioId = table.Column<int>(nullable: false),
                    ClassificationId = table.Column<int>(nullable: false),
                    GameId = table.Column<int>(nullable: false),
                    ExperienceGrowthId = table.Column<int>(nullable: false),
                    CaptureRateId = table.Column<int>(nullable: false),
                    BaseHappinessId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pokemon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pokemon_BaseHappiness_BaseHappinessId",
                        column: x => x.BaseHappinessId,
                        principalTable: "BaseHappiness",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemon_CaptureRates_CaptureRateId",
                        column: x => x.CaptureRateId,
                        principalTable: "CaptureRates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemon_Classifications_ClassificationId",
                        column: x => x.ClassificationId,
                        principalTable: "Classifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemon_EggCycles_EggCycleId",
                        column: x => x.EggCycleId,
                        principalTable: "EggCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemon_ExperienceGrowths_ExperienceGrowthId",
                        column: x => x.ExperienceGrowthId,
                        principalTable: "ExperienceGrowths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemon_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemon_GenderRatios_GenderRatioId",
                        column: x => x.GenderRatioId,
                        principalTable: "GenderRatios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShinyHunts_PokemonId",
                table: "ShinyHunts",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewedPokemons_PokemonId",
                table: "ReviewedPokemons",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTypeDetails_PokemonId",
                table: "PokemonTypeDetails",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTeamDetails_PokemonId",
                table: "PokemonTeamDetails",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLegendaryDetails_PokemonId",
                table: "PokemonLegendaryDetails",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonGameDetails_PokemonId",
                table: "PokemonGameDetails",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonFormDetails_AltFormPokemonId",
                table: "PokemonFormDetails",
                column: "AltFormPokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonFormDetails_OriginalPokemonId",
                table: "PokemonFormDetails",
                column: "OriginalPokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonEggGroupDetails_PokemonId",
                table: "PokemonEggGroupDetails",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonAbilityDetails_PokemonId",
                table: "PokemonAbilityDetails",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_GenerationId",
                table: "Games",
                column: "GenerationId");

            migrationBuilder.CreateIndex(
                name: "IX_FormItems_PokemonId",
                table: "FormItems",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_EVYields_PokemonId",
                table: "EVYields",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_EvolutionPokemonId",
                table: "Evolutions",
                column: "EvolutionPokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_PreevolutionPokemonId",
                table: "Evolutions",
                column: "PreevolutionPokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_BattleItems_GenerationId",
                table: "BattleItems",
                column: "GenerationId");

            migrationBuilder.CreateIndex(
                name: "IX_BattleItems_PokemonId",
                table: "BattleItems",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseStats_PokemonId",
                table: "BaseStats",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_BaseHappinessId",
                table: "Pokemon",
                column: "BaseHappinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_CaptureRateId",
                table: "Pokemon",
                column: "CaptureRateId");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_ClassificationId",
                table: "Pokemon",
                column: "ClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_EggCycleId",
                table: "Pokemon",
                column: "EggCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_ExperienceGrowthId",
                table: "Pokemon",
                column: "ExperienceGrowthId");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_GameId",
                table: "Pokemon",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_GenderRatioId",
                table: "Pokemon",
                column: "GenderRatioId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseStats_Pokemon_PokemonId",
                table: "BaseStats",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BattleItems_Generations_GenerationId",
                table: "BattleItems",
                column: "GenerationId",
                principalTable: "Generations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BattleItems_Pokemon_PokemonId",
                table: "BattleItems",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Evolutions_Pokemon_EvolutionPokemonId",
                table: "Evolutions",
                column: "EvolutionPokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Evolutions_Pokemon_PreevolutionPokemonId",
                table: "Evolutions",
                column: "PreevolutionPokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EVYields_Pokemon_PokemonId",
                table: "EVYields",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FormItems_Pokemon_PokemonId",
                table: "FormItems",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Generations_GenerationId",
                table: "Games",
                column: "GenerationId",
                principalTable: "Generations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonAbilityDetails_Pokemon_PokemonId",
                table: "PokemonAbilityDetails",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonEggGroupDetails_Pokemon_PokemonId",
                table: "PokemonEggGroupDetails",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonFormDetails_Pokemon_AltFormPokemonId",
                table: "PokemonFormDetails",
                column: "AltFormPokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonFormDetails_Pokemon_OriginalPokemonId",
                table: "PokemonFormDetails",
                column: "OriginalPokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonGameDetails_Pokemon_PokemonId",
                table: "PokemonGameDetails",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonLegendaryDetails_Pokemon_PokemonId",
                table: "PokemonLegendaryDetails",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeamDetails_Pokemon_PokemonId",
                table: "PokemonTeamDetails",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTypeDetails_Pokemon_PokemonId",
                table: "PokemonTypeDetails",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewedPokemons_Pokemon_PokemonId",
                table: "ReviewedPokemons",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShinyHunts_Pokemon_PokemonId",
                table: "ShinyHunts",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaseStats_Pokemon_PokemonId",
                table: "BaseStats");

            migrationBuilder.DropForeignKey(
                name: "FK_BattleItems_Generations_GenerationId",
                table: "BattleItems");

            migrationBuilder.DropForeignKey(
                name: "FK_BattleItems_Pokemon_PokemonId",
                table: "BattleItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Evolutions_Pokemon_EvolutionPokemonId",
                table: "Evolutions");

            migrationBuilder.DropForeignKey(
                name: "FK_Evolutions_Pokemon_PreevolutionPokemonId",
                table: "Evolutions");

            migrationBuilder.DropForeignKey(
                name: "FK_EVYields_Pokemon_PokemonId",
                table: "EVYields");

            migrationBuilder.DropForeignKey(
                name: "FK_FormItems_Pokemon_PokemonId",
                table: "FormItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Generations_GenerationId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonAbilityDetails_Pokemon_PokemonId",
                table: "PokemonAbilityDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonEggGroupDetails_Pokemon_PokemonId",
                table: "PokemonEggGroupDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonFormDetails_Pokemon_AltFormPokemonId",
                table: "PokemonFormDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonFormDetails_Pokemon_OriginalPokemonId",
                table: "PokemonFormDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonGameDetails_Pokemon_PokemonId",
                table: "PokemonGameDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonLegendaryDetails_Pokemon_PokemonId",
                table: "PokemonLegendaryDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeamDetails_Pokemon_PokemonId",
                table: "PokemonTeamDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTypeDetails_Pokemon_PokemonId",
                table: "PokemonTypeDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewedPokemons_Pokemon_PokemonId",
                table: "ReviewedPokemons");

            migrationBuilder.DropForeignKey(
                name: "FK_ShinyHunts_Pokemon_PokemonId",
                table: "ShinyHunts");

            migrationBuilder.DropTable(
                name: "Generations");

            migrationBuilder.DropTable(
                name: "Pokemon");

            migrationBuilder.DropIndex(
                name: "IX_ShinyHunts_PokemonId",
                table: "ShinyHunts");

            migrationBuilder.DropIndex(
                name: "IX_ReviewedPokemons_PokemonId",
                table: "ReviewedPokemons");

            migrationBuilder.DropIndex(
                name: "IX_PokemonTypeDetails_PokemonId",
                table: "PokemonTypeDetails");

            migrationBuilder.DropIndex(
                name: "IX_PokemonTeamDetails_PokemonId",
                table: "PokemonTeamDetails");

            migrationBuilder.DropIndex(
                name: "IX_PokemonLegendaryDetails_PokemonId",
                table: "PokemonLegendaryDetails");

            migrationBuilder.DropIndex(
                name: "IX_PokemonGameDetails_PokemonId",
                table: "PokemonGameDetails");

            migrationBuilder.DropIndex(
                name: "IX_PokemonFormDetails_AltFormPokemonId",
                table: "PokemonFormDetails");

            migrationBuilder.DropIndex(
                name: "IX_PokemonFormDetails_OriginalPokemonId",
                table: "PokemonFormDetails");

            migrationBuilder.DropIndex(
                name: "IX_PokemonEggGroupDetails_PokemonId",
                table: "PokemonEggGroupDetails");

            migrationBuilder.DropIndex(
                name: "IX_PokemonAbilityDetails_PokemonId",
                table: "PokemonAbilityDetails");

            migrationBuilder.DropIndex(
                name: "IX_Games_GenerationId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_FormItems_PokemonId",
                table: "FormItems");

            migrationBuilder.DropIndex(
                name: "IX_EVYields_PokemonId",
                table: "EVYields");

            migrationBuilder.DropIndex(
                name: "IX_Evolutions_EvolutionPokemonId",
                table: "Evolutions");

            migrationBuilder.DropIndex(
                name: "IX_Evolutions_PreevolutionPokemonId",
                table: "Evolutions");

            migrationBuilder.DropIndex(
                name: "IX_BattleItems_GenerationId",
                table: "BattleItems");

            migrationBuilder.DropIndex(
                name: "IX_BattleItems_PokemonId",
                table: "BattleItems");

            migrationBuilder.DropIndex(
                name: "IX_BaseStats_PokemonId",
                table: "BaseStats");
        }
    }
}
