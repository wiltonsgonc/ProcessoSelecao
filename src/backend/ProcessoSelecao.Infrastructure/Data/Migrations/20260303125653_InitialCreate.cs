using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProcessoSelecao.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessosSelecao",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VagasDisponiveis = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessosSelecao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Avaliadores",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    AreaEspecializacao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Instituicao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    ProcessoSelecaoId = table.Column<long>(type: "bigint", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avaliadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Avaliadores_ProcessosSelecao_ProcessoSelecaoId",
                        column: x => x.ProcessoSelecaoId,
                        principalTable: "ProcessosSelecao",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Candidatos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Cpf = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RG = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AreaPesquisa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StatusValidacao = table.Column<int>(type: "int", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessoSelecaoId = table.Column<long>(type: "bigint", nullable: false),
                    NumeroInscricao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: ""),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaisNatal = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EstadoNatal = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Naturalidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NomeSocial = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EstadoCivil = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Nacionalidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Sexo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Telefone2 = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CorRaca = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DataVencimentoRG = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TipoVisto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FormaInscricao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LocalProva = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CampusProva = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ValorInscricao = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    DeficienciaFisica = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeficienciaAuditiva = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeficienciaFala = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeficienciaVisual = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeficienciaMental = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeficienciaIntelectual = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeficienciaReabilitado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeficienciaMultipla = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MotivoOutrasNecessidades = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidatos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candidatos_ProcessosSelecao_ProcessoSelecaoId",
                        column: x => x.ProcessoSelecaoId,
                        principalTable: "ProcessosSelecao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Baremas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidatoId = table.Column<long>(type: "bigint", nullable: false),
                    AvaliadorId = table.Column<long>(type: "bigint", nullable: false),
                    CriteriosJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotaFinal = table.Column<float>(type: "real", nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DataPreenchimento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Baremas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Baremas_Avaliadores_AvaliadorId",
                        column: x => x.AvaliadorId,
                        principalTable: "Avaliadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Baremas_Candidatos_CandidatoId",
                        column: x => x.CandidatoId,
                        principalTable: "Candidatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documentos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    NomeArquivo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CaminhoLocal = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LinkUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HashValidacao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DataUpload = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Validado = table.Column<bool>(type: "bit", nullable: false),
                    MotivoRejeicao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CandidatoId = table.Column<long>(type: "bigint", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documentos_Candidatos_CandidatoId",
                        column: x => x.CandidatoId,
                        principalTable: "Candidatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Avaliadores_Email",
                table: "Avaliadores",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Avaliadores_ProcessoSelecaoId",
                table: "Avaliadores",
                column: "ProcessoSelecaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Baremas_AvaliadorId",
                table: "Baremas",
                column: "AvaliadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Baremas_CandidatoId",
                table: "Baremas",
                column: "CandidatoId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidatos_Email",
                table: "Candidatos",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidatos_Cpf",
                table: "Candidatos",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidatos_ProcessoSelecaoId",
                table: "Candidatos",
                column: "ProcessoSelecaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_CandidatoId",
                table: "Documentos",
                column: "CandidatoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Baremas");

            migrationBuilder.DropTable(
                name: "Documentos");

            migrationBuilder.DropTable(
                name: "Avaliadores");

            migrationBuilder.DropTable(
                name: "Candidatos");

            migrationBuilder.DropTable(
                name: "ProcessosSelecao");
        }
    }
}
