using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddTeamDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeams_Pokemon_FifthPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeams_Pokemon_FirstPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeams_Pokemon_FourthPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeams_Pokemon_SecondPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeams_Pokemon_SixthPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeams_Pokemon_ThirdPokemonId",
                table: "PokemonTeams");

            migrationBuilder.AlterColumn<int>(
                name: "ThirdPokemonId",
                table: "PokemonTeams",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SixthPokemonId",
                table: "PokemonTeams",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SecondPokemonId",
                table: "PokemonTeams",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PokemonTeamName",
                table: "PokemonTeams",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FourthPokemonId",
                table: "PokemonTeams",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FirstPokemonId",
                table: "PokemonTeams",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "FifthPokemonId",
                table: "PokemonTeams",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FromTeamRandomizer",
                table: "PokemonTeams",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PokemonTeamEVs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Health = table.Column<byte>(nullable: false),
                    Attack = table.Column<byte>(nullable: false),
                    Defense = table.Column<byte>(nullable: false),
                    SpecialAttack = table.Column<byte>(nullable: false),
                    SpecialDefense = table.Column<byte>(nullable: false),
                    Speed = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonTeamEVs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PokemonTeamIVs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Health = table.Column<byte>(nullable: false),
                    Attack = table.Column<byte>(nullable: false),
                    Defense = table.Column<byte>(nullable: false),
                    SpecialAttack = table.Column<byte>(nullable: false),
                    SpecialDefense = table.Column<byte>(nullable: false),
                    Speed = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonTeamIVs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PokemonTeamDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nickname = table.Column<string>(nullable: true),
                    IsShiny = table.Column<bool>(nullable: false),
                    Gender = table.Column<string>(maxLength: 6, nullable: true),
                    PokemonId = table.Column<string>(nullable: false),
                    PokemonTypeDetailId = table.Column<int>(nullable: false),
                    AbilityId = table.Column<int>(nullable: false),
                    PokemonTeamEVId = table.Column<int>(nullable: true),
                    PokemonTeamIVId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonTeamDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PokemonTeamDetails_Abilities_AbilityId",
                        column: x => x.AbilityId,
                        principalTable: "Abilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonTeamDetails_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonTeamDetails_PokemonTeamEVs_PokemonTeamEVId",
                        column: x => x.PokemonTeamEVId,
                        principalTable: "PokemonTeamEVs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonTeamDetails_PokemonTeamIVs_PokemonTeamIVId",
                        column: x => x.PokemonTeamIVId,
                        principalTable: "PokemonTeamIVs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonTeamDetails_PokemonTypeDetails_PokemonTypeDetailId",
                        column: x => x.PokemonTypeDetailId,
                        principalTable: "PokemonTypeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTeamDetails_AbilityId",
                table: "PokemonTeamDetails",
                column: "AbilityId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTeamDetails_PokemonId",
                table: "PokemonTeamDetails",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTeamDetails_PokemonTeamEVId",
                table: "PokemonTeamDetails",
                column: "PokemonTeamEVId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTeamDetails_PokemonTeamIVId",
                table: "PokemonTeamDetails",
                column: "PokemonTeamIVId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTeamDetails_PokemonTypeDetailId",
                table: "PokemonTeamDetails",
                column: "PokemonTypeDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_FifthPokemonId",
                table: "PokemonTeams",
                column: "FifthPokemonId",
                principalTable: "PokemonTeamDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_FirstPokemonId",
                table: "PokemonTeams",
                column: "FirstPokemonId",
                principalTable: "PokemonTeamDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_FourthPokemonId",
                table: "PokemonTeams",
                column: "FourthPokemonId",
                principalTable: "PokemonTeamDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_SecondPokemonId",
                table: "PokemonTeams",
                column: "SecondPokemonId",
                principalTable: "PokemonTeamDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_SixthPokemonId",
                table: "PokemonTeams",
                column: "SixthPokemonId",
                principalTable: "PokemonTeamDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_ThirdPokemonId",
                table: "PokemonTeams",
                column: "ThirdPokemonId",
                principalTable: "PokemonTeamDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_FifthPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_FirstPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_FourthPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_SecondPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_SixthPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonTeams_PokemonTeamDetails_ThirdPokemonId",
                table: "PokemonTeams");

            migrationBuilder.DropTable(
                name: "PokemonTeamDetails");

            migrationBuilder.DropTable(
                name: "PokemonTeamEVs");

            migrationBuilder.DropTable(
                name: "PokemonTeamIVs");

            migrationBuilder.DropColumn(
                name: "FromTeamRandomizer",
                table: "PokemonTeams");

            migrationBuilder.AlterColumn<string>(
                name: "ThirdPokemonId",
                table: "PokemonTeams",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SixthPokemonId",
                table: "PokemonTeams",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SecondPokemonId",
                table: "PokemonTeams",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PokemonTeamName",
                table: "PokemonTeams",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "FourthPokemonId",
                table: "PokemonTeams",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstPokemonId",
                table: "PokemonTeams",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "FifthPokemonId",
                table: "PokemonTeams",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeams_Pokemon_FifthPokemonId",
                table: "PokemonTeams",
                column: "FifthPokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeams_Pokemon_FirstPokemonId",
                table: "PokemonTeams",
                column: "FirstPokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeams_Pokemon_FourthPokemonId",
                table: "PokemonTeams",
                column: "FourthPokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeams_Pokemon_SecondPokemonId",
                table: "PokemonTeams",
                column: "SecondPokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeams_Pokemon_SixthPokemonId",
                table: "PokemonTeams",
                column: "SixthPokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonTeams_Pokemon_ThirdPokemonId",
                table: "PokemonTeams",
                column: "ThirdPokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
