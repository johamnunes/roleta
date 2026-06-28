using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roleta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifierNameOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationSeconds",
                table: "Modifiers");

            migrationBuilder.DropColumn(
                name: "Kind",
                table: "Modifiers");

            migrationBuilder.DropColumn(
                name: "RequiresTargetPlayer",
                table: "Modifiers");

            migrationBuilder.DropColumn(
                name: "RequiresTargetRule",
                table: "Modifiers");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "Modifiers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationSeconds",
                table: "Modifiers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Kind",
                table: "Modifiers",
                type: "TEXT",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "RequiresTargetPlayer",
                table: "Modifiers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresTargetRule",
                table: "Modifiers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Modifiers",
                type: "TEXT",
                maxLength: 280,
                nullable: false,
                defaultValue: "");
        }
    }
}
