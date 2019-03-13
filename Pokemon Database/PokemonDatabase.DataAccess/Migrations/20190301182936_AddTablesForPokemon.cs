using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PokemonDatabase.DataAccess.Migrations
{
    public partial class AddTablesForPokemon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExperienceGrowths",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    ExpPointTotal = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienceGrowths", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "GenderRatios",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MaleRatio = table.Column<double>(nullable: false),
                    FemaleRatio = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenderRatios", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Generations",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    Region = table.Column<string>(nullable: true),
                    Games = table.Column<string>(nullable: true),
                    Abbreviation = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Generations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Pokemon",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Height = table.Column<decimal>(nullable: false),
                    Weight = table.Column<decimal>(nullable: false),
                    ExpYield = table.Column<int>(nullable: false),
                    EggCycleID = table.Column<int>(nullable: true),
                    GenderRatioID = table.Column<int>(nullable: true),
                    ClassificationID = table.Column<int>(nullable: true),
                    GenerationID = table.Column<string>(nullable: true),
                    ExperienceGrowthID = table.Column<int>(nullable: true),
                    CaptureRateID = table.Column<int>(nullable: true),
                    BaseHappinessID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pokemon", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Pokemon_BaseHappiness_BaseHappinessID",
                        column: x => x.BaseHappinessID,
                        principalTable: "BaseHappiness",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemon_CaptureRates_CaptureRateID",
                        column: x => x.CaptureRateID,
                        principalTable: "CaptureRates",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemon_Classifications_ClassificationID",
                        column: x => x.ClassificationID,
                        principalTable: "Classifications",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemon_EggCycles_EggCycleID",
                        column: x => x.EggCycleID,
                        principalTable: "EggCycles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemon_ExperienceGrowths_ExperienceGrowthID",
                        column: x => x.ExperienceGrowthID,
                        principalTable: "ExperienceGrowths",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemon_GenderRatios_GenderRatioID",
                        column: x => x.GenderRatioID,
                        principalTable: "GenderRatios",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemon_Generations_GenerationID",
                        column: x => x.GenerationID,
                        principalTable: "Generations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_BaseHappinessID",
                table: "Pokemon",
                column: "BaseHappinessID");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_CaptureRateID",
                table: "Pokemon",
                column: "CaptureRateID");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_ClassificationID",
                table: "Pokemon",
                column: "ClassificationID");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_EggCycleID",
                table: "Pokemon",
                column: "EggCycleID");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_ExperienceGrowthID",
                table: "Pokemon",
                column: "ExperienceGrowthID");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_GenderRatioID",
                table: "Pokemon",
                column: "GenderRatioID");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_GenerationID",
                table: "Pokemon",
                column: "GenerationID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pokemon");

            migrationBuilder.DropTable(
                name: "ExperienceGrowths");

            migrationBuilder.DropTable(
                name: "GenderRatios");

            migrationBuilder.DropTable(
                name: "Generations");
        }
    }
}
