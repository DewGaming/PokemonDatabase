using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class RemovedCommentPagesAndCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_CommentCategories_CategoryId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_CommentPages_PageId",
                table: "Comments");

            migrationBuilder.DropTable(
                name: "CommentCategories");

            migrationBuilder.DropTable(
                name: "CommentPages");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CategoryId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_PageId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "OtherPage",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "PageId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "PokemonName",
                table: "Comments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OtherPage",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PageId",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PokemonName",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CommentCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommentPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentPages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CategoryId",
                table: "Comments",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PageId",
                table: "Comments",
                column: "PageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_CommentCategories_CategoryId",
                table: "Comments",
                column: "CategoryId",
                principalTable: "CommentCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_CommentPages_PageId",
                table: "Comments",
                column: "PageId",
                principalTable: "CommentPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
