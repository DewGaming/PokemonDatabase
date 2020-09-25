using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class MovedPokeballCatchModifierToSeparateClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CatchModifier",
                table: "Pokeballs");

            migrationBuilder.CreateTable(
                name: "PokeballCatchModifierDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PokeballId = table.Column<int>(nullable: false),
                    Effect = table.Column<string>(nullable: false),
                    CatchModifier = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokeballCatchModifierDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PokeballCatchModifierDetails_Pokeballs_PokeballId",
                        column: x => x.PokeballId,
                        principalTable: "Pokeballs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PokeballCatchModifierDetails_PokeballId",
                table: "PokeballCatchModifierDetails",
                column: "PokeballId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PokeballCatchModifierDetails");

            migrationBuilder.AddColumn<float>(
                name: "CatchModifier",
                table: "Pokeballs",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
