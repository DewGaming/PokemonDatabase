using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PokemonDatabase.DataAccess.Migrations
{
    public partial class UpdateColumnProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaseStats_Pokemon_PokemonID",
                table: "BaseStats");

            migrationBuilder.DropForeignKey(
                name: "FK_EVYields_Pokemon_PokemonID",
                table: "EVYields");

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

            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_Generations_GenerationID",
                table: "Pokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonAbilityDetails_Abilities_HiddenAbilityID",
                table: "PokemonAbilityDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonAbilityDetails_Pokemon_PokemonID",
                table: "PokemonAbilityDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonAbilityDetails_Abilities_PrimaryAbilityID",
                table: "PokemonAbilityDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonAbilityDetails_Abilities_SecondaryAbilityID",
                table: "PokemonAbilityDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTypeDetails_Pokemon_PokemonID",
                table: "PokemonTypeDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTypeDetails_Types_PrimaryTypeID",
                table: "PokemonTypeDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTypeDetails_Types_SecondaryTypeID",
                table: "PokemonTypeDetails");

            migrationBuilder.DropTable(
                name: "TypeChart");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Types",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "SecondaryTypeID",
                table: "PokemonTypeDetails",
                newName: "SecondaryTypeId");

            migrationBuilder.RenameColumn(
                name: "PrimaryTypeID",
                table: "PokemonTypeDetails",
                newName: "PrimaryTypeId");

            migrationBuilder.RenameColumn(
                name: "PokemonID",
                table: "PokemonTypeDetails",
                newName: "PokemonId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "PokemonTypeDetails",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_PokemonTypeDetails_SecondaryTypeID",
                table: "PokemonTypeDetails",
                newName: "IX_PokemonTypeDetails_SecondaryTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_PokemonTypeDetails_PrimaryTypeID",
                table: "PokemonTypeDetails",
                newName: "IX_PokemonTypeDetails_PrimaryTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_PokemonTypeDetails_PokemonID",
                table: "PokemonTypeDetails",
                newName: "IX_PokemonTypeDetails_PokemonId");

            migrationBuilder.RenameColumn(
                name: "SecondaryAbilityID",
                table: "PokemonAbilityDetails",
                newName: "SecondaryAbilityId");

            migrationBuilder.RenameColumn(
                name: "PrimaryAbilityID",
                table: "PokemonAbilityDetails",
                newName: "PrimaryAbilityId");

            migrationBuilder.RenameColumn(
                name: "PokemonID",
                table: "PokemonAbilityDetails",
                newName: "PokemonId");

            migrationBuilder.RenameColumn(
                name: "HiddenAbilityID",
                table: "PokemonAbilityDetails",
                newName: "HiddenAbilityId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "PokemonAbilityDetails",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_PokemonAbilityDetails_SecondaryAbilityID",
                table: "PokemonAbilityDetails",
                newName: "IX_PokemonAbilityDetails_SecondaryAbilityId");

            migrationBuilder.RenameIndex(
                name: "IX_PokemonAbilityDetails_PrimaryAbilityID",
                table: "PokemonAbilityDetails",
                newName: "IX_PokemonAbilityDetails_PrimaryAbilityId");

            migrationBuilder.RenameIndex(
                name: "IX_PokemonAbilityDetails_PokemonID",
                table: "PokemonAbilityDetails",
                newName: "IX_PokemonAbilityDetails_PokemonId");

            migrationBuilder.RenameIndex(
                name: "IX_PokemonAbilityDetails_HiddenAbilityID",
                table: "PokemonAbilityDetails",
                newName: "IX_PokemonAbilityDetails_HiddenAbilityId");

            migrationBuilder.RenameColumn(
                name: "GenerationID",
                table: "Pokemon",
                newName: "GenerationId");

            migrationBuilder.RenameColumn(
                name: "GenderRatioID",
                table: "Pokemon",
                newName: "GenderRatioId");

            migrationBuilder.RenameColumn(
                name: "ExperienceGrowthID",
                table: "Pokemon",
                newName: "ExperienceGrowthId");

            migrationBuilder.RenameColumn(
                name: "EggCycleID",
                table: "Pokemon",
                newName: "EggCycleId");

            migrationBuilder.RenameColumn(
                name: "ClassificationID",
                table: "Pokemon",
                newName: "ClassificationId");

            migrationBuilder.RenameColumn(
                name: "CaptureRateID",
                table: "Pokemon",
                newName: "CaptureRateId");

            migrationBuilder.RenameColumn(
                name: "BaseHappinessID",
                table: "Pokemon",
                newName: "BaseHappinessId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Pokemon",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Pokemon_GenerationID",
                table: "Pokemon",
                newName: "IX_Pokemon_GenerationId");

            migrationBuilder.RenameIndex(
                name: "IX_Pokemon_GenderRatioID",
                table: "Pokemon",
                newName: "IX_Pokemon_GenderRatioId");

            migrationBuilder.RenameIndex(
                name: "IX_Pokemon_ExperienceGrowthID",
                table: "Pokemon",
                newName: "IX_Pokemon_ExperienceGrowthId");

            migrationBuilder.RenameIndex(
                name: "IX_Pokemon_EggCycleID",
                table: "Pokemon",
                newName: "IX_Pokemon_EggCycleId");

            migrationBuilder.RenameIndex(
                name: "IX_Pokemon_ClassificationID",
                table: "Pokemon",
                newName: "IX_Pokemon_ClassificationId");

            migrationBuilder.RenameIndex(
                name: "IX_Pokemon_CaptureRateID",
                table: "Pokemon",
                newName: "IX_Pokemon_CaptureRateId");

            migrationBuilder.RenameIndex(
                name: "IX_Pokemon_BaseHappinessID",
                table: "Pokemon",
                newName: "IX_Pokemon_BaseHappinessId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Generations",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "GenderRatios",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Forms",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "ExperienceGrowths",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PokemonID",
                table: "EVYields",
                newName: "PokemonId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "EVYields",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_EVYields_PokemonID",
                table: "EVYields",
                newName: "IX_EVYields_PokemonId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "EggGroups",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "EggCycles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Classifications",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "CaptureRates",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PokemonID",
                table: "BaseStats",
                newName: "PokemonId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "BaseStats",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_BaseStats_PokemonID",
                table: "BaseStats",
                newName: "IX_BaseStats_PokemonId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "BaseHappiness",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Abilities",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Types",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PokemonId",
                table: "PokemonTypeDetails",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PokemonId",
                table: "PokemonAbilityDetails",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Pokemon",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GenerationId",
                table: "Pokemon",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                table: "Generations",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Games",
                table: "Generations",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Abbreviation",
                table: "Generations",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Forms",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ExperienceGrowths",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EggGroups",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Classifications",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Abilities",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Abilities",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BaseStats_Pokemon_PokemonId",
                table: "BaseStats",
                column: "PokemonId",
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
                name: "FK_Pokemon_BaseHappiness_BaseHappinessId",
                table: "Pokemon",
                column: "BaseHappinessId",
                principalTable: "BaseHappiness",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_CaptureRates_CaptureRateId",
                table: "Pokemon",
                column: "CaptureRateId",
                principalTable: "CaptureRates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_Classifications_ClassificationId",
                table: "Pokemon",
                column: "ClassificationId",
                principalTable: "Classifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_EggCycles_EggCycleId",
                table: "Pokemon",
                column: "EggCycleId",
                principalTable: "EggCycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_ExperienceGrowths_ExperienceGrowthId",
                table: "Pokemon",
                column: "ExperienceGrowthId",
                principalTable: "ExperienceGrowths",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_GenderRatios_GenderRatioId",
                table: "Pokemon",
                column: "GenderRatioId",
                principalTable: "GenderRatios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_Generations_GenerationId",
                table: "Pokemon",
                column: "GenerationId",
                principalTable: "Generations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonAbilityDetails_Abilities_HiddenAbilityId",
                table: "PokemonAbilityDetails",
                column: "HiddenAbilityId",
                principalTable: "Abilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonAbilityDetails_Pokemon_PokemonId",
                table: "PokemonAbilityDetails",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonAbilityDetails_Abilities_PrimaryAbilityId",
                table: "PokemonAbilityDetails",
                column: "PrimaryAbilityId",
                principalTable: "Abilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonAbilityDetails_Abilities_SecondaryAbilityId",
                table: "PokemonAbilityDetails",
                column: "SecondaryAbilityId",
                principalTable: "Abilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTypeDetails_Pokemon_PokemonId",
                table: "PokemonTypeDetails",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTypeDetails_Types_PrimaryTypeId",
                table: "PokemonTypeDetails",
                column: "PrimaryTypeId",
                principalTable: "Types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTypeDetails_Types_SecondaryTypeId",
                table: "PokemonTypeDetails",
                column: "SecondaryTypeId",
                principalTable: "Types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaseStats_Pokemon_PokemonId",
                table: "BaseStats");

            migrationBuilder.DropForeignKey(
                name: "FK_EVYields_Pokemon_PokemonId",
                table: "EVYields");

            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_BaseHappiness_BaseHappinessId",
                table: "Pokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_CaptureRates_CaptureRateId",
                table: "Pokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_Classifications_ClassificationId",
                table: "Pokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_EggCycles_EggCycleId",
                table: "Pokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_ExperienceGrowths_ExperienceGrowthId",
                table: "Pokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_GenderRatios_GenderRatioId",
                table: "Pokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_Generations_GenerationId",
                table: "Pokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonAbilityDetails_Abilities_HiddenAbilityId",
                table: "PokemonAbilityDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonAbilityDetails_Pokemon_PokemonId",
                table: "PokemonAbilityDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonAbilityDetails_Abilities_PrimaryAbilityId",
                table: "PokemonAbilityDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonAbilityDetails_Abilities_SecondaryAbilityId",
                table: "PokemonAbilityDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTypeDetails_Pokemon_PokemonId",
                table: "PokemonTypeDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTypeDetails_Types_PrimaryTypeId",
                table: "PokemonTypeDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTypeDetails_Types_SecondaryTypeId",
                table: "PokemonTypeDetails");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Types",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "SecondaryTypeId",
                table: "PokemonTypeDetails",
                newName: "SecondaryTypeID");

            migrationBuilder.RenameColumn(
                name: "PrimaryTypeId",
                table: "PokemonTypeDetails",
                newName: "PrimaryTypeID");

            migrationBuilder.RenameColumn(
                name: "PokemonId",
                table: "PokemonTypeDetails",
                newName: "PokemonID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PokemonTypeDetails",
                newName: "ID");

            migrationBuilder.RenameIndex(
                name: "IX_PokemonTypeDetails_SecondaryTypeId",
                table: "PokemonTypeDetails",
                newName: "IX_PokemonTypeDetails_SecondaryTypeID");

            migrationBuilder.RenameIndex(
                name: "IX_PokemonTypeDetails_PrimaryTypeId",
                table: "PokemonTypeDetails",
                newName: "IX_PokemonTypeDetails_PrimaryTypeID");

            migrationBuilder.RenameIndex(
                name: "IX_PokemonTypeDetails_PokemonId",
                table: "PokemonTypeDetails",
                newName: "IX_PokemonTypeDetails_PokemonID");

            migrationBuilder.RenameColumn(
                name: "SecondaryAbilityId",
                table: "PokemonAbilityDetails",
                newName: "SecondaryAbilityID");

            migrationBuilder.RenameColumn(
                name: "PrimaryAbilityId",
                table: "PokemonAbilityDetails",
                newName: "PrimaryAbilityID");

            migrationBuilder.RenameColumn(
                name: "PokemonId",
                table: "PokemonAbilityDetails",
                newName: "PokemonID");

            migrationBuilder.RenameColumn(
                name: "HiddenAbilityId",
                table: "PokemonAbilityDetails",
                newName: "HiddenAbilityID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PokemonAbilityDetails",
                newName: "ID");

            migrationBuilder.RenameIndex(
                name: "IX_PokemonAbilityDetails_SecondaryAbilityId",
                table: "PokemonAbilityDetails",
                newName: "IX_PokemonAbilityDetails_SecondaryAbilityID");

            migrationBuilder.RenameIndex(
                name: "IX_PokemonAbilityDetails_PrimaryAbilityId",
                table: "PokemonAbilityDetails",
                newName: "IX_PokemonAbilityDetails_PrimaryAbilityID");

            migrationBuilder.RenameIndex(
                name: "IX_PokemonAbilityDetails_PokemonId",
                table: "PokemonAbilityDetails",
                newName: "IX_PokemonAbilityDetails_PokemonID");

            migrationBuilder.RenameIndex(
                name: "IX_PokemonAbilityDetails_HiddenAbilityId",
                table: "PokemonAbilityDetails",
                newName: "IX_PokemonAbilityDetails_HiddenAbilityID");

            migrationBuilder.RenameColumn(
                name: "GenerationId",
                table: "Pokemon",
                newName: "GenerationID");

            migrationBuilder.RenameColumn(
                name: "GenderRatioId",
                table: "Pokemon",
                newName: "GenderRatioID");

            migrationBuilder.RenameColumn(
                name: "ExperienceGrowthId",
                table: "Pokemon",
                newName: "ExperienceGrowthID");

            migrationBuilder.RenameColumn(
                name: "EggCycleId",
                table: "Pokemon",
                newName: "EggCycleID");

            migrationBuilder.RenameColumn(
                name: "ClassificationId",
                table: "Pokemon",
                newName: "ClassificationID");

            migrationBuilder.RenameColumn(
                name: "CaptureRateId",
                table: "Pokemon",
                newName: "CaptureRateID");

            migrationBuilder.RenameColumn(
                name: "BaseHappinessId",
                table: "Pokemon",
                newName: "BaseHappinessID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Pokemon",
                newName: "ID");

            migrationBuilder.RenameIndex(
                name: "IX_Pokemon_GenerationId",
                table: "Pokemon",
                newName: "IX_Pokemon_GenerationID");

            migrationBuilder.RenameIndex(
                name: "IX_Pokemon_GenderRatioId",
                table: "Pokemon",
                newName: "IX_Pokemon_GenderRatioID");

            migrationBuilder.RenameIndex(
                name: "IX_Pokemon_ExperienceGrowthId",
                table: "Pokemon",
                newName: "IX_Pokemon_ExperienceGrowthID");

            migrationBuilder.RenameIndex(
                name: "IX_Pokemon_EggCycleId",
                table: "Pokemon",
                newName: "IX_Pokemon_EggCycleID");

            migrationBuilder.RenameIndex(
                name: "IX_Pokemon_ClassificationId",
                table: "Pokemon",
                newName: "IX_Pokemon_ClassificationID");

            migrationBuilder.RenameIndex(
                name: "IX_Pokemon_CaptureRateId",
                table: "Pokemon",
                newName: "IX_Pokemon_CaptureRateID");

            migrationBuilder.RenameIndex(
                name: "IX_Pokemon_BaseHappinessId",
                table: "Pokemon",
                newName: "IX_Pokemon_BaseHappinessID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Generations",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "GenderRatios",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Forms",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ExperienceGrowths",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "PokemonId",
                table: "EVYields",
                newName: "PokemonID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "EVYields",
                newName: "ID");

            migrationBuilder.RenameIndex(
                name: "IX_EVYields_PokemonId",
                table: "EVYields",
                newName: "IX_EVYields_PokemonID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "EggGroups",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "EggCycles",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Classifications",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CaptureRates",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "PokemonId",
                table: "BaseStats",
                newName: "PokemonID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BaseStats",
                newName: "ID");

            migrationBuilder.RenameIndex(
                name: "IX_BaseStats_PokemonId",
                table: "BaseStats",
                newName: "IX_BaseStats_PokemonID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BaseHappiness",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Abilities",
                newName: "ID");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Types",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "PokemonID",
                table: "PokemonTypeDetails",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "PokemonID",
                table: "PokemonAbilityDetails",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Pokemon",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 25);

            migrationBuilder.AlterColumn<string>(
                name: "GenerationID",
                table: "Pokemon",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                table: "Generations",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 6);

            migrationBuilder.AlterColumn<string>(
                name: "Games",
                table: "Generations",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Abbreviation",
                table: "Generations",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 5);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Forms",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 25);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ExperienceGrowths",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EggGroups",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Classifications",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 25);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Abilities",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Abilities",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 150);

            migrationBuilder.CreateTable(
                name: "TypeChart",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AttackID = table.Column<int>(nullable: true),
                    DefendID = table.Column<int>(nullable: true),
                    Effective = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeChart", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TypeChart_Types_AttackID",
                        column: x => x.AttackID,
                        principalTable: "Types",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TypeChart_Types_DefendID",
                        column: x => x.DefendID,
                        principalTable: "Types",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TypeChart_AttackID",
                table: "TypeChart",
                column: "AttackID");

            migrationBuilder.CreateIndex(
                name: "IX_TypeChart_DefendID",
                table: "TypeChart",
                column: "DefendID");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseStats_Pokemon_PokemonID",
                table: "BaseStats",
                column: "PokemonID",
                principalTable: "Pokemon",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EVYields_Pokemon_PokemonID",
                table: "EVYields",
                column: "PokemonID",
                principalTable: "Pokemon",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_Generations_GenerationID",
                table: "Pokemon",
                column: "GenerationID",
                principalTable: "Generations",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonAbilityDetails_Abilities_HiddenAbilityID",
                table: "PokemonAbilityDetails",
                column: "HiddenAbilityID",
                principalTable: "Abilities",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonAbilityDetails_Pokemon_PokemonID",
                table: "PokemonAbilityDetails",
                column: "PokemonID",
                principalTable: "Pokemon",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonAbilityDetails_Abilities_PrimaryAbilityID",
                table: "PokemonAbilityDetails",
                column: "PrimaryAbilityID",
                principalTable: "Abilities",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonAbilityDetails_Abilities_SecondaryAbilityID",
                table: "PokemonAbilityDetails",
                column: "SecondaryAbilityID",
                principalTable: "Abilities",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTypeDetails_Pokemon_PokemonID",
                table: "PokemonTypeDetails",
                column: "PokemonID",
                principalTable: "Pokemon",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTypeDetails_Types_PrimaryTypeID",
                table: "PokemonTypeDetails",
                column: "PrimaryTypeID",
                principalTable: "Types",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTypeDetails_Types_SecondaryTypeID",
                table: "PokemonTypeDetails",
                column: "SecondaryTypeID",
                principalTable: "Types",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
