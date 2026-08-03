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
        var dto = new CreateAvaliadorDto { Nome = "João", Cpf = "52998224725", Email = "joao@test.com" };
        _repositoryMock.Setup(r => r.GetByCpfAsync("52998224725")).ReturnsAsync((Avaliador?)null);

        var entity = new Avaliador { Id = 1, Nome = "João", Cpf = "52998224725" };
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
        var dto = new CreateAvaliadorDto { Nome = "João", Cpf = "52998224725" };
        var existing = new Avaliador { Id = 1, Cpf = "52998224725" };
        _repositoryMock.Setup(r => r.GetByCpfAsync("52998224725")).ReturnsAsync(existing);

        var act = () => _service.CreateAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*CPF*");
    }

    [Fact]
    public async Task CreateAsync_LancaExcecao_SeCpfInvalido()
    {
        var dto = new CreateAvaliadorDto { Nome = "João", Cpf = "12345678900", Email = "joao@test.com" };

        var act = () => _service.CreateAsync(dto);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*CPF inválido*");
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
        var entity = new Avaliador { Id = 1, Nome = "João", Cpf = "52998224725", Baremas = new List<Barema>() };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        var dto = new AvaliadorDto { Id = 1, Nome = "João" };
        _mapperMock.Setup(m => m.Map<AvaliadorDto>(entity)).Returns(dto);

        var result = await _service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_CriaAvaliadorComCamposAcademicos()
    {
        var dto = new CreateAvaliadorDto
        {
            Nome = "Maria Santos",
            Cpf = "12345678909",
            Email = "maria@test.com",
            Tipo = TipoAvaliador.Externo,
            LinkLattes = "https://lattes.cnpq.br/9876543210",
            UltimaFormacao = "Doutorado em Engenharia",
            Cargo = "Pesquisadora",
            NivelCnpq = NivelCnpq.Pq1A
        };
        _repositoryMock.Setup(r => r.GetByCpfAsync("12345678909")).ReturnsAsync((Avaliador?)null);

        var entity = new Avaliador
        {
            Id = 2,
            Nome = "Maria Santos",
            Cpf = "12345678909",
            LinkLattes = "https://lattes.cnpq.br/9876543210",
            UltimaFormacao = "Doutorado em Engenharia",
            Cargo = "Pesquisadora",
            NivelCnpq = NivelCnpq.Pq1A
        };
        _mapperMock.Setup(m => m.Map<Avaliador>(dto)).Returns(entity);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Avaliador>())).ReturnsAsync(entity);

        var dtoResult = new AvaliadorDto
        {
            Id = 2,
            Nome = "Maria Santos",
            LinkLattes = "https://lattes.cnpq.br/9876543210",
            UltimaFormacao = "Doutorado em Engenharia",
            Cargo = "Pesquisadora",
            NivelCnpq = NivelCnpq.Pq1A
        };
        _mapperMock.Setup(m => m.Map<AvaliadorDto>(entity)).Returns(dtoResult);

        var result = await _service.CreateAsync(dto);

        result.Should().NotBeNull();
        result.Nome.Should().Be("Maria Santos");
        result.LinkLattes.Should().Be("https://lattes.cnpq.br/9876543210");
        result.UltimaFormacao.Should().Be("Doutorado em Engenharia");
        result.Cargo.Should().Be("Pesquisadora");
        result.NivelCnpq.Should().Be(NivelCnpq.Pq1A);
    }

    [Fact]
    public async Task CreateAsync_DefineNivelCnpqDefault_SeNaoInformado()
    {
        var dto = new CreateAvaliadorDto
        {
            Nome = "Carlos Oliveira",
            Cpf = "11122233396",
            Email = "carlos@test.com"
        };
        _repositoryMock.Setup(r => r.GetByCpfAsync("11122233396")).ReturnsAsync((Avaliador?)null);

        var entity = new Avaliador { Id = 3, Nome = "Carlos Oliveira", Cpf = "11122233396" };
        _mapperMock.Setup(m => m.Map<Avaliador>(dto)).Returns(entity);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Avaliador>())).ReturnsAsync(entity);

        var dtoResult = new AvaliadorDto { Id = 3, Nome = "Carlos Oliveira", NivelCnpq = NivelCnpq.NaoSeAplica };
        _mapperMock.Setup(m => m.Map<AvaliadorDto>(entity)).Returns(dtoResult);

        var result = await _service.CreateAsync(dto);

        result.Should().NotBeNull();
        result.NivelCnpq.Should().Be(NivelCnpq.NaoSeAplica);
    }

    [Fact]
    public async Task UpdateAsync_AtualizaCamposAcademicos()
    {
        var entity = new Avaliador
        {
            Id = 1,
            Nome = "João",
            Cpf = "52998224725",
            LinkLattes = "https://lattes.cnpq.br/antigo",
            UltimaFormacao = "Mestrado",
            Cargo = "Professor",
            NivelCnpq = NivelCnpq.Pq2
        };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        var dto = new UpdateAvaliadorDto
        {
            Nome = "João Silva",
            LinkLattes = "https://lattes.cnpq.br/novo",
            UltimaFormacao = "Doutorado",
            Cargo = "Professor Associado",
            NivelCnpq = NivelCnpq.Pq1C,
            Ativo = true
        };

        var updatedEntity = new Avaliador
        {
            Id = 1,
            Nome = "João Silva",
            LinkLattes = "https://lattes.cnpq.br/novo",
            UltimaFormacao = "Doutorado",
            Cargo = "Professor Associado",
            NivelCnpq = NivelCnpq.Pq1C
        };
        _mapperMock.Setup(m => m.Map(dto, entity)).Returns(updatedEntity);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Avaliador>())).ReturnsAsync(updatedEntity);

        var dtoResult = new AvaliadorDto
        {
            Id = 1,
            Nome = "João Silva",
            LinkLattes = "https://lattes.cnpq.br/novo",
            UltimaFormacao = "Doutorado",
            Cargo = "Professor Associado",
            NivelCnpq = NivelCnpq.Pq1C
        };
        _mapperMock.Setup(m => m.Map<AvaliadorDto>(updatedEntity)).Returns(dtoResult);

        var result = await _service.UpdateAsync(1, dto);

        result.Should().NotBeNull();
        result.LinkLattes.Should().Be("https://lattes.cnpq.br/novo");
        result.UltimaFormacao.Should().Be("Doutorado");
        result.Cargo.Should().Be("Professor Associado");
        result.NivelCnpq.Should().Be(NivelCnpq.Pq1C);
    }
}
