using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roleta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifierKind : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Kind",
                table: "Modifiers",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "Flip");

            // Melhor esforço: nomes antigos do Rulette → kinds canônicos.
            migrationBuilder.Sql("""
                UPDATE Modifiers SET Kind = 'Swap'  WHERE lower(Name) IN ('swap', 'troca');
                UPDATE Modifiers SET Kind = 'Clone' WHERE lower(Name) IN ('clone', 'ctrl+c ctrl+v', 'ctrl c / ctrl v');
                UPDATE Modifiers SET Kind = 'Flip'  WHERE lower(Name) IN ('flip', 'inversão', 'inversao');
                UPDATE Modifiers SET Kind = 'Left'  WHERE lower(Name) IN ('esquerda', 'left');
                UPDATE Modifiers SET Kind = 'Right' WHERE lower(Name) IN ('direita', 'right');
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Modifiers_Kind",
                table: "Modifiers",
                column: "Kind");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Modifiers_Kind",
                table: "Modifiers");

            migrationBuilder.DropColumn(
                name: "Kind",
                table: "Modifiers");
        }
    }
}
