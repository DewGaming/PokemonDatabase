using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedFormGroupDetailTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormGroupGameDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(nullable: false),
                    FormGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormGroupGameDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormGroupGameDetails_FormGroups_FormGroupId",
                        column: x => x.FormGroupId,
                        principalTable: "FormGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormGroupGameDetails_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormGroupGameDetails_FormGroupId",
                table: "FormGroupGameDetails",
                column: "FormGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_FormGroupGameDetails_GameId",
                table: "FormGroupGameDetails",
                column: "GameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormGroupGameDetails");
        }
    }
}
