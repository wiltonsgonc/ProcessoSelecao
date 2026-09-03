using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProcessoSelecao.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBaremaTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TemplateId",
                table: "Baremas",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BaremaTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TipoBarema = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PontoMaximo = table.Column<float>(type: "real", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    CriadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaremaTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaremaTemplateItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateId = table.Column<long>(type: "bigint", nullable: false),
                    Secao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SecaoOrdem = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ordem = table.Column<int>(type: "int", nullable: false),
                    NotaMinima = table.Column<float>(type: "real", nullable: false),
                    NotaMaxima = table.Column<float>(type: "real", nullable: false),
                    Passo = table.Column<float>(type: "real", nullable: false),
                    Obrigatorio = table.Column<bool>(type: "bit", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaremaTemplateItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaremaTemplateItems_BaremaTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "BaremaTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BaremaItensAvaliacao",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaremaId = table.Column<long>(type: "bigint", nullable: false),
                    TemplateItemId = table.Column<long>(type: "bigint", nullable: false),
                    Nota = table.Column<float>(type: "real", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaremaItensAvaliacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaremaItensAvaliacao_BaremaTemplateItems_TemplateItemId",
                        column: x => x.TemplateItemId,
                        principalTable: "BaremaTemplateItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaremaItensAvaliacao_Baremas_BaremaId",
                        column: x => x.BaremaId,
                        principalTable: "Baremas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Baremas_TemplateId",
                table: "Baremas",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_BaremaItensAvaliacao_BaremaId",
                table: "BaremaItensAvaliacao",
                column: "BaremaId");

            migrationBuilder.CreateIndex(
                name: "IX_BaremaItensAvaliacao_TemplateItemId",
                table: "BaremaItensAvaliacao",
                column: "TemplateItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BaremaTemplateItems_TemplateId",
                table: "BaremaTemplateItems",
                column: "TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Baremas_BaremaTemplates_TemplateId",
                table: "Baremas",
                column: "TemplateId",
                principalTable: "BaremaTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Baremas_BaremaTemplates_TemplateId",
                table: "Baremas");

            migrationBuilder.DropTable(
                name: "BaremaItensAvaliacao");

            migrationBuilder.DropTable(
                name: "BaremaTemplateItems");

            migrationBuilder.DropTable(
                name: "BaremaTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Baremas_TemplateId",
                table: "Baremas");

            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "Baremas");
        }
    }
}
