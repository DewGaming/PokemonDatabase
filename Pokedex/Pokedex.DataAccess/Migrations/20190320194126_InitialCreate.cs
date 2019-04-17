using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Abilities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 20, nullable: false),
                    Description = table.Column<string>(maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Abilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaseHappiness",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Happiness = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseHappiness", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CaptureRates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CatchRate = table.Column<short>(nullable: false),
                    ChanceOfCapture = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaptureRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Classifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EggCycles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CycleCount = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EggCycles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EggGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EggGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EvolutionMethods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvolutionMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExperienceGrowths",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 15, nullable: false),
                    ExpPointTotal = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienceGrowths", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GenderRatios",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MaleRatio = table.Column<double>(nullable: false),
                    FemaleRatio = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenderRatios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Generations",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 4, nullable: false),
                    Region = table.Column<string>(maxLength: 6, nullable: false),
                    Games = table.Column<string>(maxLength: 50, nullable: false),
                    Abbreviation = table.Column<string>(maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Generations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Types",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pokemon",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 6, nullable: false),
                    Name = table.Column<string>(maxLength: 25, nullable: false),
                    Height = table.Column<decimal>(nullable: false),
                    Weight = table.Column<decimal>(nullable: false),
                    ExpYield = table.Column<int>(nullable: false),
                    EggCycleId = table.Column<int>(nullable: false),
                    GenderRatioId = table.Column<int>(nullable: false),
                    ClassificationId = table.Column<int>(nullable: false),
                    GenerationId = table.Column<string>(nullable: false),
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
                        name: "FK_Pokemon_GenderRatios_GenderRatioId",
                        column: x => x.GenderRatioId,
                        principalTable: "GenderRatios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemon_Generations_GenerationId",
                        column: x => x.GenerationId,
                        principalTable: "Generations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TypeCharts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Effective = table.Column<decimal>(nullable: false),
                    AttackId = table.Column<int>(nullable: false),
                    DefendId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeCharts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TypeCharts_Types_AttackId",
                        column: x => x.AttackId,
                        principalTable: "Types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TypeCharts_Types_DefendId",
                        column: x => x.DefendId,
                        principalTable: "Types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BaseStats",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Health = table.Column<short>(nullable: false),
                    Attack = table.Column<short>(nullable: false),
                    Defense = table.Column<short>(nullable: false),
                    SpecialAttack = table.Column<short>(nullable: false),
                    SpecialDefense = table.Column<short>(nullable: false),
                    Speed = table.Column<short>(nullable: false),
                    StatTotal = table.Column<short>(nullable: false),
                    PokemonId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaseStats_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Evolutions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EvolutionDetails = table.Column<string>(maxLength: 200, nullable: true),
                    EvolutionMethodId = table.Column<int>(nullable: false),
                    PreevolutionPokemonId = table.Column<string>(nullable: false),
                    EvolutionPokemonId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evolutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evolutions_EvolutionMethods_EvolutionMethodId",
                        column: x => x.EvolutionMethodId,
                        principalTable: "EvolutionMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Evolutions_Pokemon_EvolutionPokemonId",
                        column: x => x.EvolutionPokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Evolutions_Pokemon_PreevolutionPokemonId",
                        column: x => x.PreevolutionPokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EVYields",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Health = table.Column<short>(nullable: false),
                    Attack = table.Column<short>(nullable: false),
                    Defense = table.Column<short>(nullable: false),
                    SpecialAttack = table.Column<short>(nullable: false),
                    SpecialDefense = table.Column<short>(nullable: false),
                    Speed = table.Column<short>(nullable: false),
                    EVTotal = table.Column<short>(nullable: false),
                    PokemonId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVYields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EVYields_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PokemonAbilityDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PokemonId = table.Column<string>(nullable: false),
                    PrimaryAbilityId = table.Column<int>(nullable: false),
                    SecondaryAbilityId = table.Column<int>(nullable: true),
                    HiddenAbilityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonAbilityDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PokemonAbilityDetails_Abilities_HiddenAbilityId",
                        column: x => x.HiddenAbilityId,
                        principalTable: "Abilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonAbilityDetails_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonAbilityDetails_Abilities_PrimaryAbilityId",
                        column: x => x.PrimaryAbilityId,
                        principalTable: "Abilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonAbilityDetails_Abilities_SecondaryAbilityId",
                        column: x => x.SecondaryAbilityId,
                        principalTable: "Abilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PokemonEggGroupDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PokemonId = table.Column<string>(nullable: false),
                    PrimaryEggGroupId = table.Column<int>(nullable: false),
                    SecondaryEggGroupId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonEggGroupDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PokemonEggGroupDetails_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonEggGroupDetails_EggGroups_PrimaryEggGroupId",
                        column: x => x.PrimaryEggGroupId,
                        principalTable: "EggGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonEggGroupDetails_EggGroups_SecondaryEggGroupId",
                        column: x => x.SecondaryEggGroupId,
                        principalTable: "EggGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PokemonFormDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FormId = table.Column<int>(nullable: false),
                    OriginalPokemonId = table.Column<string>(nullable: false),
                    AltFormPokemonId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonFormDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PokemonFormDetails_Pokemon_AltFormPokemonId",
                        column: x => x.AltFormPokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonFormDetails_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonFormDetails_Pokemon_OriginalPokemonId",
                        column: x => x.OriginalPokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PokemonTypeDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PokemonId = table.Column<string>(nullable: false),
                    PrimaryTypeId = table.Column<int>(nullable: false),
                    SecondaryTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonTypeDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PokemonTypeDetails_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonTypeDetails_Types_PrimaryTypeId",
                        column: x => x.PrimaryTypeId,
                        principalTable: "Types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonTypeDetails_Types_SecondaryTypeId",
                        column: x => x.SecondaryTypeId,
                        principalTable: "Types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaseStats_PokemonId",
                table: "BaseStats",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_EvolutionMethodId",
                table: "Evolutions",
                column: "EvolutionMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_EvolutionPokemonId",
                table: "Evolutions",
                column: "EvolutionPokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_PreevolutionPokemonId",
                table: "Evolutions",
                column: "PreevolutionPokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_EVYields_PokemonId",
                table: "EVYields",
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
                name: "IX_Pokemon_GenderRatioId",
                table: "Pokemon",
                column: "GenderRatioId");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_GenerationId",
                table: "Pokemon",
                column: "GenerationId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonAbilityDetails_HiddenAbilityId",
                table: "PokemonAbilityDetails",
                column: "HiddenAbilityId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonAbilityDetails_PokemonId",
                table: "PokemonAbilityDetails",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonAbilityDetails_PrimaryAbilityId",
                table: "PokemonAbilityDetails",
                column: "PrimaryAbilityId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonAbilityDetails_SecondaryAbilityId",
                table: "PokemonAbilityDetails",
                column: "SecondaryAbilityId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonEggGroupDetails_PokemonId",
                table: "PokemonEggGroupDetails",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonEggGroupDetails_PrimaryEggGroupId",
                table: "PokemonEggGroupDetails",
                column: "PrimaryEggGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonEggGroupDetails_SecondaryEggGroupId",
                table: "PokemonEggGroupDetails",
                column: "SecondaryEggGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonFormDetails_AltFormPokemonId",
                table: "PokemonFormDetails",
                column: "AltFormPokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonFormDetails_FormId",
                table: "PokemonFormDetails",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonFormDetails_OriginalPokemonId",
                table: "PokemonFormDetails",
                column: "OriginalPokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTypeDetails_PokemonId",
                table: "PokemonTypeDetails",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTypeDetails_PrimaryTypeId",
                table: "PokemonTypeDetails",
                column: "PrimaryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTypeDetails_SecondaryTypeId",
                table: "PokemonTypeDetails",
                column: "SecondaryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TypeCharts_AttackId",
                table: "TypeCharts",
                column: "AttackId");

            migrationBuilder.CreateIndex(
                name: "IX_TypeCharts_DefendId",
                table: "TypeCharts",
                column: "DefendId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaseStats");

            migrationBuilder.DropTable(
                name: "Evolutions");

            migrationBuilder.DropTable(
                name: "EVYields");

            migrationBuilder.DropTable(
                name: "PokemonAbilityDetails");

            migrationBuilder.DropTable(
                name: "PokemonEggGroupDetails");

            migrationBuilder.DropTable(
                name: "PokemonFormDetails");

            migrationBuilder.DropTable(
                name: "PokemonTypeDetails");

            migrationBuilder.DropTable(
                name: "TypeCharts");

            migrationBuilder.DropTable(
                name: "EvolutionMethods");

            migrationBuilder.DropTable(
                name: "Abilities");

            migrationBuilder.DropTable(
                name: "EggGroups");

            migrationBuilder.DropTable(
                name: "Forms");

            migrationBuilder.DropTable(
                name: "Pokemon");

            migrationBuilder.DropTable(
                name: "Types");

            migrationBuilder.DropTable(
                name: "BaseHappiness");

            migrationBuilder.DropTable(
                name: "CaptureRates");

            migrationBuilder.DropTable(
                name: "Classifications");

            migrationBuilder.DropTable(
                name: "EggCycles");

            migrationBuilder.DropTable(
                name: "ExperienceGrowths");

            migrationBuilder.DropTable(
                name: "GenderRatios");

            migrationBuilder.DropTable(
                name: "Generations");
        }
    }
}
