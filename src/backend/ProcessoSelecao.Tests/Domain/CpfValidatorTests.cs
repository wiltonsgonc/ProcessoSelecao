using FluentAssertions;
using ProcessoSelecao.Domain.Helpers;

namespace ProcessoSelecao.Tests.Domain;

public class CpfValidatorTests
{
    [Theory]
    [InlineData("52998224725")]
    [InlineData("12345678909")]
    [InlineData("11122233396")]
    [InlineData("98765432100")]
    [InlineData("55566677720")]
    public void IsValid_CpfValido_DeveRetornarVerdadeiro(string cpf)
    {
        CpfValidator.IsValid(cpf).Should().BeTrue();
    }

    [Theory]
    [InlineData("529.982.247-25")]
    [InlineData("123.456.789-09")]
    [InlineData("111.222.333-96")]
    public void IsValid_CpfFormatado_DeveRetornarVerdadeiro(string cpf)
    {
        CpfValidator.IsValid(cpf).Should().BeTrue();
    }

    [Theory]
    [InlineData("11111111111")]
    [InlineData("22222222222")]
    [InlineData("33333333333")]
    [InlineData("44444444444")]
    [InlineData("55555555555")]
    [InlineData("66666666666")]
    [InlineData("77777777777")]
    [InlineData("88888888888")]
    [InlineData("99999999999")]
    [InlineData("00000000000")]
    public void IsValid_CpfComDigitosIguais_DeveRetornarFalso(string cpf)
    {
        CpfValidator.IsValid(cpf).Should().BeFalse();
    }

    [Theory]
    [InlineData("12345678900")]
    [InlineData("52998224726")]
    [InlineData("98765432101")]
    [InlineData("11122233305")]
    public void IsValid_CpfComDigitosVerificadoresErrados_DeveRetornarFalso(string cpf)
    {
        CpfValidator.IsValid(cpf).Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("123")]
    [InlineData("1234567890")]
    [InlineData("123456789012")]
    public void IsValid_CpfComTamanhoInvalido_DeveRetornarFalso(string? cpf)
    {
        CpfValidator.IsValid(cpf).Should().BeFalse();
    }

    [Fact]
    public void Clean_DeveRemoverPontosTraçosEEspaços()
    {
        CpfValidator.Clean("529.982.247-25").Should().Be("52998224725");
        CpfValidator.Clean("529 982 247 25").Should().Be("52998224725");
        CpfValidator.Clean("529-982-247-25").Should().Be("52998224725");
    }

    [Fact]
    public void Clean_DeveRetornarVazioParaNuloOuBranco()
    {
        CpfValidator.Clean(null).Should().BeEmpty();
        CpfValidator.Clean("").Should().BeEmpty();
        CpfValidator.Clean("   ").Should().BeEmpty();
    }

    [Fact]
    public void Format_DeveFormatarCorretamente()
    {
        CpfValidator.Format("52998224725").Should().Be("529.982.247-25");
        CpfValidator.Format("12345678909").Should().Be("123.456.789-09");
    }

    [Fact]
    public void Format_DeveFormatarCpfComCaracteresExtras()
    {
        CpfValidator.Format("529.982.247-25").Should().Be("529.982.247-25");
        CpfValidator.Format("529 982 247 25").Should().Be("529.982.247-25");
    }

    [Fact]
    public void Format_DeveRetornarOriginalSeTamanhoInvalido()
    {
        CpfValidator.Format("123").Should().Be("123");
        CpfValidator.Format("").Should().Be("");
    }
}
