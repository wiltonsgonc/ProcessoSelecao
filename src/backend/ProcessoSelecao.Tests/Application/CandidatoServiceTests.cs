using AutoMapper;
using FluentAssertions;
using Moq;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Application.Services;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Tests.Application;

public class CandidatoServiceTests
{
    private readonly Mock<ICandidatoRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CandidatoService _service;

    public CandidatoServiceTests()
    {
        _repositoryMock = new Mock<ICandidatoRepository>();
        _mapperMock = new Mock<IMapper>();
        _service = new CandidatoService(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateAsync_CriaCandidato_ComDadosValidos()
    {
        var dto = new CreateCandidatoDto { Nome = "Maria", Cpf = "52998224725", Email = "maria@test.com", ProcessoSelecaoId = 1 };
        var entity = new Candidato { Id = 1, Nome = "Maria", Cpf = "52998224725", Email = "maria@test.com" };
        var dtoResult = new CandidatoDto { Id = 1, Nome = "Maria" };

        _mapperMock.Setup(m => m.Map<Candidato>(dto)).Returns(entity);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Candidato>())).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<CandidatoDto>(entity)).Returns(dtoResult);

        var result = await _service.CreateAsync(dto);

        result.Should().NotBeNull();
        result.Nome.Should().Be("Maria");
        entity.DataCadastro.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        entity.StatusValidacao.Should().Be(StatusValidacao.Pendente);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Candidato>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_RetornaTodosCandidatos()
    {
        var candidatos = new List<Candidato>
        {
            new() { Id = 1, Nome = "Maria", Cpf = "52998224725" },
            new() { Id = 2, Nome = "Joao", Cpf = "12345678909" }
        };
        var dtos = new List<CandidatoDto>
        {
            new() { Id = 1, Nome = "Maria" },
            new() { Id = 2, Nome = "Joao" }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(candidatos);
        _mapperMock.Setup(m => m.Map<CandidatoDto>(candidatos[0])).Returns(dtos[0]);
        _mapperMock.Setup(m => m.Map<CandidatoDto>(candidatos[1])).Returns(dtos[1]);

        var result = (await _service.GetAllAsync()).ToList();

        result.Should().HaveCount(2);
        result[0].Nome.Should().Be("Maria");
        result[1].Nome.Should().Be("Joao");
    }

    [Fact]
    public async Task GetByIdAsync_RetornaCandidato_SeEncontrado()
    {
        var entity = new Candidato { Id = 1, Nome = "Maria", Cpf = "52998224725" };
        var dto = new CandidatoDto { Id = 1, Nome = "Maria" };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<CandidatoDto>(entity)).Returns(dto);

        var result = await _service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Nome.Should().Be("Maria");
    }

    [Fact]
    public async Task GetByIdAsync_RetornaNull_SeNaoEncontrado()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Candidato?)null);

        var result = await _service.GetByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_AtualizaCandidato_SeEncontrado()
    {
        var entity = new Candidato { Id = 1, Nome = "Maria", AreaPesquisa = "TI" };
        var dto = new UpdateCandidatoDto { Nome = "Maria Santos", AreaPesquisa = "Saude" };
        var updatedEntity = new Candidato { Id = 1, Nome = "Maria Santos", AreaPesquisa = "Saude" };
        var dtoResult = new CandidatoDto { Id = 1, Nome = "Maria Santos" };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map(dto, entity));
        _repositoryMock.Setup(r => r.UpdateAsync(entity)).ReturnsAsync(updatedEntity);
        _mapperMock.Setup(m => m.Map<CandidatoDto>(updatedEntity)).Returns(dtoResult);

        var result = await _service.UpdateAsync(1, dto);

        result.Should().NotBeNull();
        result.Nome.Should().Be("Maria Santos");
    }

    [Fact]
    public async Task UpdateAsync_LancaException_SeNaoEncontrado()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Candidato?)null);
        var dto = new UpdateCandidatoDto { Nome = "Teste" };

        var act = () => _service.UpdateAsync(999, dto);

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*não encontrado*");
    }

    [Fact]
    public async Task DeleteAsync_RemoveCandidato()
    {
        _repositoryMock.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        await _service.DeleteAsync(1);

        _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetByProcessoIdAsync_RetornaCandidatosDoProcesso()
    {
        var candidatos = new List<Candidato>
        {
            new() { Id = 1, Nome = "Maria", ProcessoSelecaoId = 1 },
            new() { Id = 2, Nome = "Joao", ProcessoSelecaoId = 1 }
        };
        var dtos = new List<CandidatoDto>
        {
            new() { Id = 1, Nome = "Maria" },
            new() { Id = 2, Nome = "Joao" }
        };

        _repositoryMock.Setup(r => r.GetByProcessoIdAsync(1)).ReturnsAsync(candidatos);
        _mapperMock.Setup(m => m.Map<CandidatoDto>(candidatos[0])).Returns(dtos[0]);
        _mapperMock.Setup(m => m.Map<CandidatoDto>(candidatos[1])).Returns(dtos[1]);

        var result = (await _service.GetByProcessoIdAsync(1)).ToList();

        result.Should().HaveCount(2);
        result.All(c => c.Nome == "Maria" || c.Nome == "Joao").Should().BeTrue();
    }

    [Fact]
    public async Task GetPontuacaoAsync_RetornaPontuacao_DoCandidato()
    {
        var entity = new Candidato
        {
            Id = 1,
            Nome = "Maria",
            Baremas = new List<Barema>
            {
                new() { NotaFinal = 8.0f, Status = StatusBarema.Concluido },
                new() { NotaFinal = 9.0f, Status = StatusBarema.Concluido }
            }
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        var result = await _service.GetPontuacaoAsync(1);

        result.Should().Be(8.5f);
    }

    [Fact]
    public async Task GetPontuacaoAsync_LancaException_SeNaoEncontrado()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Candidato?)null);

        var act = () => _service.GetPontuacaoAsync(999);

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*não encontrado*");
    }
}
