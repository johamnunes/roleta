using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roleta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GameEndingScript : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EndingScript",
                table: "Games",
                type: "TEXT",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndingScript",
                table: "Games");
        }
    }
}
