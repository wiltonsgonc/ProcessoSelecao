using AutoMapper;
using FluentAssertions;
using Moq;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Application.Services;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Tests.Application;

public class AvaliadorServiceTests
{
    private readonly Mock<IAvaliadorRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AvaliadorService _service;

    public AvaliadorServiceTests()
    {
        _repositoryMock = new Mock<IAvaliadorRepository>();
        _mapperMock = new Mock<IMapper>();
        _service = new AvaliadorService(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateAsync_CriaAvaliador_SeCpfNaoExiste()
    {
        var dto = new CreateAvaliadorDto { Nome = "João", Cpf = "12345678900", Email = "joao@test.com" };
        _repositoryMock.Setup(r => r.GetByCpfAsync("12345678900")).ReturnsAsync((Avaliador?)null);

        var entity = new Avaliador { Id = 1, Nome = "João", Cpf = "12345678900" };
        _mapperMock.Setup(m => m.Map<Avaliador>(dto)).Returns(entity);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Avaliador>())).ReturnsAsync(entity);

        var dtoResult = new AvaliadorDto { Id = 1, Nome = "João" };
        _mapperMock.Setup(m => m.Map<AvaliadorDto>(entity)).Returns(dtoResult);

        var result = await _service.CreateAsync(dto);

        result.Should().NotBeNull();
        result.Nome.Should().Be("João");
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Avaliador>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_LancaExcecao_SeCpfJaExiste()
    {
        var dto = new CreateAvaliadorDto { Nome = "João", Cpf = "12345678900" };
        var existing = new Avaliador { Id = 1, Cpf = "12345678900" };
        _repositoryMock.Setup(r => r.GetByCpfAsync("12345678900")).ReturnsAsync(existing);

        var act = () => _service.CreateAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*CPF*");
    }

    [Fact]
    public async Task GetByIdAsync_RetornaNull_SeNaoEncontrado()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Avaliador?)null);

        var result = await _service.GetByIdAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_RetornaDto_SeEncontrado()
    {
        var entity = new Avaliador { Id = 1, Nome = "João", Cpf = "12345678900", Baremas = new List<Barema>() };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        var dto = new AvaliadorDto { Id = 1, Nome = "João" };
        _mapperMock.Setup(m => m.Map<AvaliadorDto>(entity)).Returns(dto);

        var result = await _service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }
}
