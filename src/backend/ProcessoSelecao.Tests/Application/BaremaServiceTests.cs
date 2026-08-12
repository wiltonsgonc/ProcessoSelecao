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
    private readonly Mock<IMapper> _mapperMock;
    private readonly BaremaService _service;

    public BaremaServiceTests()
    {
        _repositoryMock = new Mock<IBaremaRepository>();
        _avaliadorRepositoryMock = new Mock<IAvaliadorRepository>();
        _candidatoRepositoryMock = new Mock<ICandidatoRepository>();
        _mapperMock = new Mock<IMapper>();
        _service = new BaremaService(
            _repositoryMock.Object,
            _avaliadorRepositoryMock.Object,
            _candidatoRepositoryMock.Object,
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
    public async Task CreateAsync_LancaExcecao_SeAvaliadorMesmoCPF()
    {
        var dto = new CreateBaremaDto { CandidatoId = 1, AvaliadorId = 2 };
        var avaliador = new Avaliador { Id = 2, Cpf = "12345678900" };
        var candidato = new Candidato { Id = 1, Cpf = "12345678900" };

        _avaliadorRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(avaliador);
        _candidatoRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(candidato);

        var act = () => _service.CreateAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*avaliador não pode avaliar*mesmo CPF*");
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
}
