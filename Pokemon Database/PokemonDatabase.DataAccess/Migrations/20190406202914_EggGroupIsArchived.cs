using Microsoft.EntityFrameworkCore.Migrations;

namespace PokemonDatabase.DataAccess.Migrations
{
    public partial class EggGroupIsArchived : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "EggGroups",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "EggGroups");
        }
    }
}
