using AutoMapper;
using FluentAssertions;
using Moq;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Application.Services;
using ProcessoSelecao.Domain.Entities;
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
    public async Task CreateAsync_CriaCandidato_SeCpfValidoENaoExiste()
    {
        var dto = new CreateCandidatoDto { Nome = "Maria", Cpf = "52998224725", Email = "maria@test.com" };
        _repositoryMock.Setup(r => r.GetByCpfAsync("52998224725")).ReturnsAsync((Candidato?)null);

        var entity = new Candidato { Id = 1, Nome = "Maria", Cpf = "52998224725" };
        _mapperMock.Setup(m => m.Map<Candidato>(dto)).Returns(entity);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Candidato>())).ReturnsAsync(entity);

        var dtoResult = new CandidatoDto { Id = 1, Nome = "Maria" };
        _mapperMock.Setup(m => m.Map<CandidatoDto>(entity)).Returns(dtoResult);

        var result = await _service.CreateAsync(dto);

        result.Should().NotBeNull();
        result.Nome.Should().Be("Maria");
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Candidato>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SalvaCpfLimpo()
    {
        var dto = new CreateCandidatoDto { Nome = "Maria", Cpf = "529.982.247-25", Email = "maria@test.com" };
        _repositoryMock.Setup(r => r.GetByCpfAsync("52998224725")).ReturnsAsync((Candidato?)null);

        var entity = new Candidato { Id = 1, Nome = "Maria", Cpf = "529.982.247-25" };
        _mapperMock.Setup(m => m.Map<Candidato>(dto)).Returns(entity);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Candidato>())).ReturnsAsync(entity);

        _mapperMock.Setup(m => m.Map<CandidatoDto>(entity)).Returns(new CandidatoDto { Id = 1, Nome = "Maria" });

        await _service.CreateAsync(dto);

        entity.Cpf.Should().Be("52998224725");
    }

    [Fact]
    public async Task CreateAsync_LancaArgumentException_SeCpfInvalido()
    {
        var dto = new CreateCandidatoDto { Nome = "Maria", Cpf = "12345678900", Email = "maria@test.com" };

        var act = () => _service.CreateAsync(dto);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*CPF inválido*");
        _repositoryMock.Verify(r => r.GetByCpfAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_LancaArgumentException_SeCpfVazio()
    {
        var dto = new CreateCandidatoDto { Nome = "Maria", Cpf = "", Email = "maria@test.com" };

        var act = () => _service.CreateAsync(dto);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*CPF inválido*");
    }

    [Fact]
    public async Task CreateAsync_LancaInscricaoDuplicadaException_SeCpfJaExiste()
    {
        var dto = new CreateCandidatoDto { Nome = "Maria", Cpf = "52998224725", Email = "maria@test.com" };
        var existing = new Candidato { Id = 1, Cpf = "52998224725" };
        _repositoryMock.Setup(r => r.GetByCpfAsync("52998224725")).ReturnsAsync(existing);

        var act = () => _service.CreateAsync(dto);

        await act.Should().ThrowAsync<InscricaoDuplicadaException>()
            .WithMessage("*CPF*");
    }

    [Fact]
    public async Task GetByInscricaoECPFAsync_RetornaCandidato_SeEncontrado()
    {
        var entity = new Candidato { Id = 1, Nome = "Maria", Cpf = "52998224725", NumeroInscricao = "202600100001" };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Candidato> { entity });

        var dtoResult = new CandidatoDto { Id = 1, Nome = "Maria", Cpf = "52998224725" };
        _mapperMock.Setup(m => m.Map<CandidatoDto>(entity)).Returns(dtoResult);

        var result = await _service.GetByInscricaoECPFAsync("202600100001", "529.982.247-25");

        result.Should().NotBeNull();
        result!.Nome.Should().Be("Maria");
    }

    [Fact]
    public async Task GetByInscricaoECPFAsync_RetornaNull_SeNaoEncontrado()
    {
        _repositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Candidato> { new() { Id = 1, Cpf = "12345678909", NumeroInscricao = "202600100002" } });

        var result = await _service.GetByInscricaoECPFAsync("202600100001", "52998224725");

        result.Should().BeNull();
    }
}
