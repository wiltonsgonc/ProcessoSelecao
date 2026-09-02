using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Application.Services;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Tests.Application;

public class DocumentoServiceTests
{
    private readonly Mock<IDocumentoRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly DocumentoService _service;

    public DocumentoServiceTests()
    {
        _repositoryMock = new Mock<IDocumentoRepository>();
        _mapperMock = new Mock<IMapper>();
        var config = new Mock<IConfiguration>();
        config.Setup(c => c["Storage:CaminhoBase"]).Returns("/tmp/test");
        _service = new DocumentoService(_repositoryMock.Object, _mapperMock.Object, config.Object);
    }

    [Fact]
    public async Task GetByProcessoIdAsync_RetornaDocumentos_DoProcesso()
    {
        var documentos = new List<Documento>
        {
            new() { Id = 1, CandidatoId = 10, NomeArquivo = "doc1.pdf", Candidato = new Candidato { Nome = "João" } },
            new() { Id = 2, CandidatoId = 11, NomeArquivo = "doc2.pdf", Candidato = new Candidato { Nome = "Maria" } }
        };

        var dtos = new List<DocumentoDto>
        {
            new() { Id = 1, NomeArquivo = "doc1.pdf" },
            new() { Id = 2, NomeArquivo = "doc2.pdf" }
        };

        _repositoryMock.Setup(r => r.GetByProcessoIdAsync(1)).ReturnsAsync(documentos);
        _mapperMock.Setup(m => m.Map<IEnumerable<DocumentoDto>>(documentos)).Returns(dtos);

        var result = (await _service.GetByProcessoIdAsync(1)).ToList();

        result.Should().HaveCount(2);
        result[0].CandidatoNome.Should().Be("João");
        result[1].CandidatoNome.Should().Be("Maria");
    }

    [Fact]
    public async Task GetByProcessoIdAsync_RetornaVazio_SeNenhumDocumento()
    {
        _repositoryMock.Setup(r => r.GetByProcessoIdAsync(99))
            .ReturnsAsync(new List<Documento>());

        var result = (await _service.GetByProcessoIdAsync(99)).ToList();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByCandidatoIdAsync_RetornaDocumentos_DoCandidato()
    {
        var documentos = new List<Documento>
        {
            new() { Id = 1, CandidatoId = 10, NomeArquivo = "historico.pdf" }
        };

        var dtos = new List<DocumentoDto>
        {
            new() { Id = 1, NomeArquivo = "historico.pdf" }
        };

        _repositoryMock.Setup(r => r.GetByCandidatoIdAsync(10)).ReturnsAsync(documentos);
        _mapperMock.Setup(m => m.Map<IEnumerable<DocumentoDto>>(documentos)).Returns(dtos);

        var result = (await _service.GetByCandidatoIdAsync(10)).ToList();

        result.Should().HaveCount(1);
        result[0].NomeArquivo.Should().Be("historico.pdf");
    }

    [Fact]
    public async Task DeleteAsync_DeletaDocumento_SeEncontrado()
    {
        var doc = new Documento { Id = 1, CaminhoLocal = "/tmp/test/doc.pdf" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(doc);
        _repositoryMock.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        await _service.DeleteAsync(1);

        _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task ValidateAsync_ValidaDocumento()
    {
        var doc = new Documento { Id = 1, Validado = false };
        var updated = new Documento { Id = 1, Validado = true };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(doc);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Documento>())).ReturnsAsync(updated);
        _mapperMock.Setup(m => m.Map<DocumentoDto>(It.IsAny<Documento>()))
            .Returns(new DocumentoDto { Id = 1, Validado = true });

        var result = await _service.ValidateAsync(1, new ValidateDocumentoDto { Validado = true });

        result.Should().NotBeNull();
        result.Validado.Should().BeTrue();
    }
}
