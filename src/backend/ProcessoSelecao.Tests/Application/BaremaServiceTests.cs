using AutoMapper;
using FluentAssertions;
using Moq;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Application.Services;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Tests.Application;

public class BaremaServiceTests
{
    private readonly Mock<IBaremaRepository> _repositoryMock;
    private readonly Mock<IAvaliadorRepository> _avaliadorRepositoryMock;
    private readonly Mock<ICandidatoRepository> _candidatoRepositoryMock;
    private readonly Mock<IDocumentoRepository> _documentoRepositoryMock;
    private readonly Mock<IBaremaTemplateRepository> _templateRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly BaremaService _service;

    public BaremaServiceTests()
    {
        _repositoryMock = new Mock<IBaremaRepository>();
        _avaliadorRepositoryMock = new Mock<IAvaliadorRepository>();
        _candidatoRepositoryMock = new Mock<ICandidatoRepository>();
        _documentoRepositoryMock = new Mock<IDocumentoRepository>();
        _templateRepositoryMock = new Mock<IBaremaTemplateRepository>();
        _mapperMock = new Mock<IMapper>();
        _service = new BaremaService(
            _repositoryMock.Object,
            _candidatoRepositoryMock.Object,
            _avaliadorRepositoryMock.Object,
            _documentoRepositoryMock.Object,
            _templateRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task CreateAsync_CriaBarema_SeCPFDiferente()
    {
        var dto = new CreateBaremaDto { CandidatoId = 1, AvaliadorId = 2 };
        var avaliador = new Avaliador { Id = 2, Cpf = "11111111111" };
        var candidato = new Candidato { Id = 1, Cpf = "22222222222" };

        _avaliadorRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(avaliador);
        _candidatoRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(candidato);

        var entity = new Barema { Id = 1, CandidatoId = 1, AvaliadorId = 2, Status = StatusBarema.Pendente };
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Barema>())).ReturnsAsync(entity);

        var dtoResult = new BaremaDto { Id = 1, CandidatoId = 1, AvaliadorId = 2 };
        _mapperMock.Setup(m => m.Map<BaremaDto>(entity)).Returns(dtoResult);

        var result = await _service.CreateAsync(dto);

        result.Should().NotBeNull();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Barema>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_LancaExcecao_SeAvaliadorMesmoOrientador()
    {
        var dto = new CreateBaremaDto { CandidatoId = 1, AvaliadorId = 2 };
        var avaliador = new Avaliador { Id = 2, Nome = "Dr. Pedro", Cpf = "11111111111" };
        var candidato = new Candidato { Id = 1, Cpf = "22222222222", Orientador = "Dr. Pedro" };

        _avaliadorRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(avaliador);
        _candidatoRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(candidato);

        var act = () => _service.CreateAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*avaliador não pode avaliar*");
    }

    [Fact]
    public async Task FinalizarAsync_CalculaNotaEStatus()
    {
        var entity = new Barema { Id = 1, Status = StatusBarema.EmPreenchimento };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Barema>())).ReturnsAsync(entity);

        var dtoResult = new BaremaDto { Id = 1, NotaFinal = 7.5f, Status = StatusBarema.Concluido };
        _mapperMock.Setup(m => m.Map<BaremaDto>(It.IsAny<Barema>())).Returns(dtoResult);

        var dto = new FinalizarBaremaDto
        {
            Criterios = new Dictionary<string, float> { { "originalidade", 8 }, { "relevancia", 7 } },
            Observacoes = "Boa avaliação"
        };

        var result = await _service.FinalizarAsync(1, dto);

        result.Should().NotBeNull();
        result.Status.Should().Be(StatusBarema.Concluido);
        entity.Status.Should().Be(StatusBarema.Concluido);
        entity.DataPreenchimento.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateCriteriosAsync_LancaExcecao_SeJaFinalizado()
    {
        var entity = new Barema { Id = 1, Status = StatusBarema.Concluido };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        var dto = new UpdateBaremaDto
        {
            Criterios = new Dictionary<string, float> { { "originalidade", 8 } }
        };

        var act = () => _service.UpdateCriteriosAsync(1, dto);

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*já foi finalizado*");
    }

    [Fact]
    public async Task GetByIdAsync_RetornaNull_SeNaoEncontrado()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Barema?)null);

        var result = await _service.GetByIdAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetDadosBaremaAsync_RetornaDados_SeBaremaExiste()
    {
        var avaliador = new Avaliador { Id = 20, Nome = "Dr. Pedro" };
        var barema = new Barema { Id = 1, CandidatoId = 10, AvaliadorId = 20, TipoBarema = "PIBIC", Avaliador = avaliador };
        var candidato = new Candidato { Id = 10, Nome = "João Silva", Orientador = "Prof. Maria" };
        var documentos = new List<Documento> { new() { Id = 1, NomeArquivo = "doc.pdf" } };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(barema);
        _candidatoRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(candidato);
        _avaliadorRepositoryMock.Setup(r => r.GetByIdAsync(20)).ReturnsAsync(avaliador);
        _documentoRepositoryMock.Setup(r => r.GetByCandidatoIdAsync(10)).ReturnsAsync(documentos);
        _mapperMock.Setup(m => m.Map<DocumentoDto>(It.IsAny<Documento>()))
            .Returns(new DocumentoDto { Id = 1, NomeArquivo = "doc.pdf" });

        var result = await _service.GetDadosBaremaAsync(1);

        result.Should().NotBeNull();
        result!.BaremaId.Should().Be(1);
        result.NomeEstudante.Should().Be("João Silva");
        result.NomeOrientador.Should().Be("Prof. Maria");
        result.NomeAvaliador.Should().Be("Dr. Pedro");
    }

    [Fact]
    public async Task GetDadosBaremaAsync_RetornaNull_SeBaremaNaoExiste()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Barema?)null);

        var result = await _service.GetDadosBaremaAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task FinalizarPorEliminacaoAsync_FinalizaComZero()
    {
        var entity = new Barema { Id = 1, Status = StatusBarema.EmPreenchimento, NotaFinal = 5, TipoBarema = "PIBIC" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Barema>())).ReturnsAsync(entity);

        var dtoResult = new BaremaDto { Id = 1, NotaFinal = 0, Status = StatusBarema.Concluido };
        _mapperMock.Setup(m => m.Map<BaremaDto>(It.IsAny<Barema>())).Returns(dtoResult);

        var result = await _service.FinalizarPorEliminacaoAsync(1);

        result.Should().NotBeNull();
        entity.Observacoes.Should().Contain("eliminado");
    }

    [Fact]
    public async Task FinalizarPorEliminacaoAsync_LancaExcecao_SeNaoEncontrado()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Barema?)null);

        var act = () => _service.FinalizarPorEliminacaoAsync(99);

        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task GetProgressoAsync_ComProcessoId_RetornaProgresso()
    {
        var candidatos = new List<Candidato>
        {
            new() { Id = 1, Nome = "João", NumeroInscricao = "001" }
        };
        var baremas = new List<Barema>
        {
            new() { Id = 10, CandidatoId = 1, AvaliadorId = 20, Status = StatusBarema.Concluido, NotaFinal = 8 }
        };

        _candidatoRepositoryMock.Setup(r => r.GetByProcessoIdAsync(1)).ReturnsAsync(candidatos);
        _repositoryMock.Setup(r => r.GetByCandidatoIdAsync(1)).ReturnsAsync(baremas);

        var result = (await _service.GetProgressoAsync(1)).ToList();

        result.Should().HaveCount(1);
        result[0].CandidatoNome.Should().Be("João");
        result[0].AvaliadoresConcluidos.Should().Be(1);
        result[0].NotaFinal.Should().Be(8);
    }

    [Fact]
    public async Task GetProgressoAsync_SemProcessoId_RetornaTodosProcessos()
    {
        var baremas = new List<Barema>
        {
            new() { Id = 10, CandidatoId = 1, AvaliadorId = 20, Status = StatusBarema.Concluido, NotaFinal = 8 },
            new() { Id = 11, CandidatoId = 1, AvaliadorId = 21, Status = StatusBarema.Pendente, NotaFinal = 0 }
        };
        var candidato = new Candidato { Id = 1, Nome = "João", NumeroInscricao = "001" };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(baremas);
        _candidatoRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(candidato);

        var result = (await _service.GetProgressoAsync()).ToList();

        result.Should().HaveCount(1);
        result[0].AvaliadoresAtribuidos.Should().Be(2);
        result[0].AvaliadoresConcluidos.Should().Be(1);
    }
}
