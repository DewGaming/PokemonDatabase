using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddeSpecialGroupingsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpecialGroupingId",
                table: "Pokemon",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SpecialGroupings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialGroupings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_SpecialGroupingId",
                table: "Pokemon",
                column: "SpecialGroupingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_SpecialGroupings_SpecialGroupingId",
                table: "Pokemon",
                column: "SpecialGroupingId",
                principalTable: "SpecialGroupings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_SpecialGroupings_SpecialGroupingId",
                table: "Pokemon");

            migrationBuilder.DropTable(
                name: "SpecialGroupings");

            migrationBuilder.DropIndex(
                name: "IX_Pokemon_SpecialGroupingId",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "SpecialGroupingId",
                table: "Pokemon");
        }
    }
}
