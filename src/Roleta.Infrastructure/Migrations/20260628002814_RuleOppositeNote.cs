using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roleta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RuleOppositeNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OppositeJudgeNotes",
                table: "Rules",
                type: "TEXT",
                maxLength: 280,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OppositeJudgeNotes",
                table: "Rules");
        }
    }
}
