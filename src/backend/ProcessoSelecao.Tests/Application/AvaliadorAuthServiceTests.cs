using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using ProcessoSelecao.Application.Services;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Tests.Application;

public class AvaliadorAuthServiceTests
{
    private readonly Mock<IAvaliadorRepository> _repositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AvaliadorAuthService _service;

    public AvaliadorAuthServiceTests()
    {
        _repositoryMock = new Mock<IAvaliadorRepository>();
        _configurationMock = new Mock<IConfiguration>();
        _service = new AvaliadorAuthService(_repositoryMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task LoginAsync_RetornaToken_SeCpfESenhaValidos()
    {
        var senha = "MinhaSenh@123";
        var hash = BCrypt.Net.BCrypt.HashPassword(senha);
        var avaliador = new Avaliador
        {
            Id = 1,
            Nome = "João",
            Email = "joao@test.com",
            Cpf = "12345678900",
            Ativo = true,
            SenhaHash = hash
        };

        _repositoryMock.Setup(r => r.GetByCpfAsync("12345678900")).ReturnsAsync(avaliador);
        _configurationMock.Setup(c => c["JwtSettings:SecretKey"]).Returns("ProcessoSelecao_SecretKey_Minimo32Caracteres_2026!");
        _configurationMock.Setup(c => c["JwtSettings:Issuer"]).Returns("ProcessoSelecaoApi");
        _configurationMock.Setup(c => c["JwtSettings:Audience"]).Returns("ProcessoSelecaoWeb");

        var result = await _service.LoginAsync("12345678900", senha);

        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.AvaliadorId.Should().Be(1);
        result.Nome.Should().Be("João");
    }

    [Fact]
    public async Task LoginAsync_RetornaNull_SeCpfNaoExiste()
    {
        _repositoryMock.Setup(r => r.GetByCpfAsync("00000000000")).ReturnsAsync((Avaliador?)null);

        var result = await _service.LoginAsync("00000000000", "qualquer");

        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_RetornaNull_SeSenhaIncorreta()
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("SenhaCorreta");
        var avaliador = new Avaliador
        {
            Id = 1,
            Cpf = "12345678900",
            Ativo = true,
            SenhaHash = hash
        };

        _repositoryMock.Setup(r => r.GetByCpfAsync("12345678900")).ReturnsAsync(avaliador);

        var result = await _service.LoginAsync("12345678900", "SenhaErrada");

        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_RetornaNull_SeAvaliadorInativo()
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("Senha123");
        var avaliador = new Avaliador
        {
            Id = 1,
            Cpf = "12345678900",
            Ativo = false,
            SenhaHash = hash
        };

        _repositoryMock.Setup(r => r.GetByCpfAsync("12345678900")).ReturnsAsync(avaliador);

        var result = await _service.LoginAsync("12345678900", "Senha123");

        result.Should().BeNull();
    }

    [Fact]
    public async Task DefinirSenhaAsync_GeraHash_BCrypt()
    {
        var avaliador = new Avaliador { Id = 1, Nome = "João" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(avaliador);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Avaliador>())).ReturnsAsync(avaliador);

        await _service.DefinirSenhaAsync(1, "MinhaSenh@123");

        avaliador.SenhaHash.Should().NotBeNullOrEmpty();
        BCrypt.Net.BCrypt.Verify("MinhaSenh@123", avaliador.SenhaHash!).Should().BeTrue();
    }

    [Fact]
    public async Task DefinirSenhaAsync_LancaExcecao_SeAvaliadorNaoEncontrado()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Avaliador?)null);

        var act = () => _service.DefinirSenhaAsync(99, "senha");

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*não encontrado*");
    }
}
