using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedAlcremieSweetsForShinyHunts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SweetId",
                table: "ShinyHunts",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Sweets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sweets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShinyHunts_SweetId",
                table: "ShinyHunts",
                column: "SweetId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShinyHunts_Sweets_SweetId",
                table: "ShinyHunts",
                column: "SweetId",
                principalTable: "Sweets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShinyHunts_Sweets_SweetId",
                table: "ShinyHunts");

            migrationBuilder.DropTable(
                name: "Sweets");

            migrationBuilder.DropIndex(
                name: "IX_ShinyHunts_SweetId",
                table: "ShinyHunts");

            migrationBuilder.DropColumn(
                name: "SweetId",
                table: "ShinyHunts");
        }
    }
}
