using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProcessoSelecao.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoProcessoOrientadorTipoBarema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoProcesso",
                table: "ProcessosSelecao",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Orientador",
                table: "Candidatos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoBarema",
                table: "Baremas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoProcesso",
                table: "ProcessosSelecao");

            migrationBuilder.DropColumn(
                name: "Orientador",
                table: "Candidatos");

            migrationBuilder.DropColumn(
                name: "TipoBarema",
                table: "Baremas");
        }
    }
}
