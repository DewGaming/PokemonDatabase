using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class UpdatedCommentClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentType",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "CommentedPage",
                table: "Comments");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Comments",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PageId",
                table: "Comments",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_CommentCategories_CategoryId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_CommentPages_PageId",
                table: "Comments");

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
                name: "PageId",
                table: "Comments");

            migrationBuilder.AddColumn<string>(
                name: "CommentType",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CommentedPage",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
