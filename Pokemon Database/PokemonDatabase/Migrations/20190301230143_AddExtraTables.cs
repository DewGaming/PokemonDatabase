using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PokemonDatabase.Migrations
{
    public partial class AddExtraTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_BaseHappiness_BaseHappinessID",
                table: "Pokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_CaptureRates_CaptureRateID",
                table: "Pokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_Classifications_ClassificationID",
                table: "Pokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_EggCycles_EggCycleID",
                table: "Pokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_ExperienceGrowths_ExperienceGrowthID",
                table: "Pokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_GenderRatios_GenderRatioID",
                table: "Pokemon");

            migrationBuilder.AlterColumn<int>(
                name: "GenderRatioID",
                table: "Pokemon",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ExperienceGrowthID",
                table: "Pokemon",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EggCycleID",
                table: "Pokemon",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ClassificationID",
                table: "Pokemon",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CaptureRateID",
                table: "Pokemon",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BaseHappinessID",
                table: "Pokemon",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "BaseStats",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Health = table.Column<short>(nullable: false),
                    Attack = table.Column<short>(nullable: false),
                    Defense = table.Column<short>(nullable: false),
                    SpecialAttack = table.Column<short>(nullable: false),
                    SpecialDefense = table.Column<short>(nullable: false),
                    Speed = table.Column<short>(nullable: false),
                    StatTotal = table.Column<short>(nullable: false),
                    PokemonID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseStats", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BaseStats_Pokemon_PokemonID",
                        column: x => x.PokemonID,
                        principalTable: "Pokemon",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EVYields",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Health = table.Column<short>(nullable: false),
                    Attack = table.Column<short>(nullable: false),
                    Defense = table.Column<short>(nullable: false),
                    SpecialAttack = table.Column<short>(nullable: false),
                    SpecialDefense = table.Column<short>(nullable: false),
                    Speed = table.Column<short>(nullable: false),
                    EVTotal = table.Column<short>(nullable: false),
                    PokemonID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVYields", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EVYields_Pokemon_PokemonID",
                        column: x => x.PokemonID,
                        principalTable: "Pokemon",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaseStats_PokemonID",
                table: "BaseStats",
                column: "PokemonID");

            migrationBuilder.CreateIndex(
                name: "IX_EVYields_PokemonID",
                table: "EVYields",
                column: "PokemonID");

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_BaseHappiness_BaseHappinessID",
                table: "Pokemon",
                column: "BaseHappinessID",
                principalTable: "BaseHappiness",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_CaptureRates_CaptureRateID",
                table: "Pokemon",
                column: "CaptureRateID",
                principalTable: "CaptureRates",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_Classifications_ClassificationID",
                table: "Pokemon",
                column: "ClassificationID",
                principalTable: "Classifications",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_EggCycles_EggCycleID",
                table: "Pokemon",
                column: "EggCycleID",
                principalTable: "EggCycles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_ExperienceGrowths_ExperienceGrowthID",
                table: "Pokemon",
                column: "ExperienceGrowthID",
                principalTable: "ExperienceGrowths",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_GenderRatios_GenderRatioID",
                table: "Pokemon",
                column: "GenderRatioID",
                principalTable: "GenderRatios",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_BaseHappiness_BaseHappinessID",
                table: "Pokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_CaptureRates_CaptureRateID",
                table: "Pokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_Classifications_ClassificationID",
                table: "Pokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_EggCycles_EggCycleID",
                table: "Pokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_ExperienceGrowths_ExperienceGrowthID",
                table: "Pokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_GenderRatios_GenderRatioID",
                table: "Pokemon");

            migrationBuilder.DropTable(
                name: "BaseStats");

            migrationBuilder.DropTable(
                name: "EVYields");

            migrationBuilder.DropTable(
                name: "Forms");

            migrationBuilder.AlterColumn<int>(
                name: "GenderRatioID",
                table: "Pokemon",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ExperienceGrowthID",
                table: "Pokemon",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "EggCycleID",
                table: "Pokemon",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ClassificationID",
                table: "Pokemon",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "CaptureRateID",
                table: "Pokemon",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "BaseHappinessID",
                table: "Pokemon",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_BaseHappiness_BaseHappinessID",
                table: "Pokemon",
                column: "BaseHappinessID",
                principalTable: "BaseHappiness",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_CaptureRates_CaptureRateID",
                table: "Pokemon",
                column: "CaptureRateID",
                principalTable: "CaptureRates",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_Classifications_ClassificationID",
                table: "Pokemon",
                column: "ClassificationID",
                principalTable: "Classifications",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_EggCycles_EggCycleID",
                table: "Pokemon",
                column: "EggCycleID",
                principalTable: "EggCycles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_ExperienceGrowths_ExperienceGrowthID",
                table: "Pokemon",
                column: "ExperienceGrowthID",
                principalTable: "ExperienceGrowths",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_GenderRatios_GenderRatioID",
                table: "Pokemon",
                column: "GenderRatioID",
                principalTable: "GenderRatios",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
