using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddedFormGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OptionName",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "SeparateRandomize",
                table: "Forms");

            migrationBuilder.AddColumn<int>(
                name: "FormGroupId",
                table: "Forms",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FormGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormGroups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Forms_FormGroupId",
                table: "Forms",
                column: "FormGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_FormGroups_FormGroupId",
                table: "Forms",
                column: "FormGroupId",
                principalTable: "FormGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forms_FormGroups_FormGroupId",
                table: "Forms");

            migrationBuilder.DropTable(
                name: "FormGroups");

            migrationBuilder.DropIndex(
                name: "IX_Forms_FormGroupId",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "FormGroupId",
                table: "Forms");

            migrationBuilder.AddColumn<string>(
                name: "OptionName",
                table: "Forms",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SeparateRandomize",
                table: "Forms",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
