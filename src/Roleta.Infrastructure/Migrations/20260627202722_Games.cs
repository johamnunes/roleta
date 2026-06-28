using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roleta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Games : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Player1 = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    Player2 = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    Player3 = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActionDefinitionGame",
                columns: table => new
                {
                    ActionsId = table.Column<Guid>(type: "TEXT", nullable: false),
                    GameId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionDefinitionGame", x => new { x.ActionsId, x.GameId });
                    table.ForeignKey(
                        name: "FK_ActionDefinitionGame_Actions_ActionsId",
                        column: x => x.ActionsId,
                        principalTable: "Actions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActionDefinitionGame_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameModifierDefinition",
                columns: table => new
                {
                    GameId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModifiersId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameModifierDefinition", x => new { x.GameId, x.ModifiersId });
                    table.ForeignKey(
                        name: "FK_GameModifierDefinition_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameModifierDefinition_Modifiers_ModifiersId",
                        column: x => x.ModifiersId,
                        principalTable: "Modifiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameRuleDefinition",
                columns: table => new
                {
                    GameId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RulesId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameRuleDefinition", x => new { x.GameId, x.RulesId });
                    table.ForeignKey(
                        name: "FK_GameRuleDefinition_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameRuleDefinition_Rules_RulesId",
                        column: x => x.RulesId,
                        principalTable: "Rules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionDefinitionGame_GameId",
                table: "ActionDefinitionGame",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameModifierDefinition_ModifiersId",
                table: "GameModifierDefinition",
                column: "ModifiersId");

            migrationBuilder.CreateIndex(
                name: "IX_GameRuleDefinition_RulesId",
                table: "GameRuleDefinition",
                column: "RulesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionDefinitionGame");

            migrationBuilder.DropTable(
                name: "GameModifierDefinition");

            migrationBuilder.DropTable(
                name: "GameRuleDefinition");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
