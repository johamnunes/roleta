using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roleta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GameBasePoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BasePoints",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasePoints",
                table: "Games");
        }
    }
}
