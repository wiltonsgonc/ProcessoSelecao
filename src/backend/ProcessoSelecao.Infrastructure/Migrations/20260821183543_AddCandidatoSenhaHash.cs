using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProcessoSelecao.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidatoSenhaHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SenhaHash",
                table: "Candidatos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenhaHash",
                table: "Candidatos");
        }
    }
}
