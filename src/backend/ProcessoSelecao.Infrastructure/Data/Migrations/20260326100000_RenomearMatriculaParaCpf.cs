using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProcessoSelecao.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenomearMatriculaParaCpf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Matricula",
                table: "Candidatos",
                newName: "Cpf");

            migrationBuilder.RenameIndex(
                name: "IX_Candidatos_Matricula",
                table: "Candidatos",
                newName: "IX_Candidatos_Cpf");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Cpf",
                table: "Candidatos",
                newName: "Matricula");

            migrationBuilder.RenameIndex(
                name: "IX_Candidatos_Cpf",
                table: "Candidatos",
                newName: "IX_Candidatos_Matricula");
        }
    }
}