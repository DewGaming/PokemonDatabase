using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokedex.DataAccess.Migrations
{
    public partial class AddShinyHuntingTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShinyHuntingTechniques",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Abbreviation = table.Column<string>(maxLength: 5, nullable: false),
                    Technique = table.Column<string>(nullable: false),
                    IsArchived = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShinyHuntingTechniques", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserShinyHuntingTechniqueDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ShinyHuntingTechniqueId = table.Column<int>(nullable: false),
                    PokemonId = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    GenerationId = table.Column<string>(nullable: false),
                    ShinyAttemptCount = table.Column<int>(nullable: false),
                    HasShinyCharm = table.Column<bool>(nullable: false),
                    IsPokemonCaught = table.Column<bool>(nullable: false),
                    IsArchived = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserShinyHuntingTechniqueDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserShinyHuntingTechniqueDetails_Generations_GenerationId",
                        column: x => x.GenerationId,
                        principalTable: "Generations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserShinyHuntingTechniqueDetails_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserShinyHuntingTechniqueDetails_ShinyHuntingTechniques_ShinyHuntingTechniqueId",
                        column: x => x.ShinyHuntingTechniqueId,
                        principalTable: "ShinyHuntingTechniques",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserShinyHuntingTechniqueDetails_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserShinyHuntingTechniqueDetails_GenerationId",
                table: "UserShinyHuntingTechniqueDetails",
                column: "GenerationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserShinyHuntingTechniqueDetails_PokemonId",
                table: "UserShinyHuntingTechniqueDetails",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_UserShinyHuntingTechniqueDetails_ShinyHuntingTechniqueId",
                table: "UserShinyHuntingTechniqueDetails",
                column: "ShinyHuntingTechniqueId");

            migrationBuilder.CreateIndex(
                name: "IX_UserShinyHuntingTechniqueDetails_UserId",
                table: "UserShinyHuntingTechniqueDetails",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserShinyHuntingTechniqueDetails");

            migrationBuilder.DropTable(
                name: "ShinyHuntingTechniques");
        }
    }
}
