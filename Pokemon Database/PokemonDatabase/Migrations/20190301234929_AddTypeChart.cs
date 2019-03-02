using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PokemonDatabase.Migrations
{
    public partial class AddTypeChart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TypeChart",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Effective = table.Column<decimal>(nullable: false),
                    AttackID = table.Column<int>(nullable: true),
                    DefendID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeChart", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TypeChart_Types_AttackID",
                        column: x => x.AttackID,
                        principalTable: "Types",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TypeChart_Types_DefendID",
                        column: x => x.DefendID,
                        principalTable: "Types",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TypeChart_AttackID",
                table: "TypeChart",
                column: "AttackID");

            migrationBuilder.CreateIndex(
                name: "IX_TypeChart_DefendID",
                table: "TypeChart",
                column: "DefendID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TypeChart");
        }
    }
}
