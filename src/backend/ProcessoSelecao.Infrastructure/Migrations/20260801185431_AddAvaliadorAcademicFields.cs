using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProcessoSelecao.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAvaliadorAcademicFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cargo",
                table: "Avaliadores",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkLattes",
                table: "Avaliadores",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NivelCnpq",
                table: "Avaliadores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UltimaFormacao",
                table: "Avaliadores",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cargo",
                table: "Avaliadores");

            migrationBuilder.DropColumn(
                name: "LinkLattes",
                table: "Avaliadores");

            migrationBuilder.DropColumn(
                name: "NivelCnpq",
                table: "Avaliadores");

            migrationBuilder.DropColumn(
                name: "UltimaFormacao",
                table: "Avaliadores");
        }
    }
}
