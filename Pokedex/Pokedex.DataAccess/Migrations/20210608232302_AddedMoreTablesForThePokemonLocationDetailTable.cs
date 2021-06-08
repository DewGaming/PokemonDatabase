using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedMoreTablesForThePokemonLocationDetailTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CaptureMethodId",
                table: "PokemonLocationDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaximumLevel",
                table: "PokemonLocationDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinimumLevel",
                table: "PokemonLocationDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SOSBattleOnly",
                table: "PokemonLocationDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SeasonId",
                table: "PokemonLocationDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimeId",
                table: "PokemonLocationDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WeatherId",
                table: "PokemonLocationDetails",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CaptureMethods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaptureMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Times",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Times", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Weathers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weathers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLocationDetails_CaptureMethodId",
                table: "PokemonLocationDetails",
                column: "CaptureMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLocationDetails_SeasonId",
                table: "PokemonLocationDetails",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLocationDetails_TimeId",
                table: "PokemonLocationDetails",
                column: "TimeId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonLocationDetails_WeatherId",
                table: "PokemonLocationDetails",
                column: "WeatherId");

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonLocationDetails_CaptureMethods_CaptureMethodId",
                table: "PokemonLocationDetails",
                column: "CaptureMethodId",
                principalTable: "CaptureMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonLocationDetails_Seasons_SeasonId",
                table: "PokemonLocationDetails",
                column: "SeasonId",
                principalTable: "Seasons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonLocationDetails_Times_TimeId",
                table: "PokemonLocationDetails",
                column: "TimeId",
                principalTable: "Times",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PokemonLocationDetails_Weathers_WeatherId",
                table: "PokemonLocationDetails",
                column: "WeatherId",
                principalTable: "Weathers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PokemonLocationDetails_CaptureMethods_CaptureMethodId",
                table: "PokemonLocationDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonLocationDetails_Seasons_SeasonId",
                table: "PokemonLocationDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonLocationDetails_Times_TimeId",
                table: "PokemonLocationDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PokemonLocationDetails_Weathers_WeatherId",
                table: "PokemonLocationDetails");

            migrationBuilder.DropTable(
                name: "CaptureMethods");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropTable(
                name: "Times");

            migrationBuilder.DropTable(
                name: "Weathers");

            migrationBuilder.DropIndex(
                name: "IX_PokemonLocationDetails_CaptureMethodId",
                table: "PokemonLocationDetails");

            migrationBuilder.DropIndex(
                name: "IX_PokemonLocationDetails_SeasonId",
                table: "PokemonLocationDetails");

            migrationBuilder.DropIndex(
                name: "IX_PokemonLocationDetails_TimeId",
                table: "PokemonLocationDetails");

            migrationBuilder.DropIndex(
                name: "IX_PokemonLocationDetails_WeatherId",
                table: "PokemonLocationDetails");

            migrationBuilder.DropColumn(
                name: "CaptureMethodId",
                table: "PokemonLocationDetails");

            migrationBuilder.DropColumn(
                name: "MaximumLevel",
                table: "PokemonLocationDetails");

            migrationBuilder.DropColumn(
                name: "MinimumLevel",
                table: "PokemonLocationDetails");

            migrationBuilder.DropColumn(
                name: "SOSBattleOnly",
                table: "PokemonLocationDetails");

            migrationBuilder.DropColumn(
                name: "SeasonId",
                table: "PokemonLocationDetails");

            migrationBuilder.DropColumn(
                name: "TimeId",
                table: "PokemonLocationDetails");

            migrationBuilder.DropColumn(
                name: "WeatherId",
                table: "PokemonLocationDetails");
        }
    }
}
