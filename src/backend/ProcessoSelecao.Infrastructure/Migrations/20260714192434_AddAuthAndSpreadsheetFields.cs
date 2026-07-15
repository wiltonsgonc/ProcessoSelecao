using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProcessoSelecao.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthAndSpreadsheetFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SenhaHash",
                table: "Avaliadores",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenhaHash",
                table: "Avaliadores");
        }
    }
}
