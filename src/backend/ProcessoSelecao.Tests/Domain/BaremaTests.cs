using FluentAssertions;
using ProcessoSelecao.Domain.Entities;

namespace ProcessoSelecao.Tests.Domain;

public class BaremaTests
{
    [Fact]
    public void CalcularNotaFinal_RetornaZeroSeNull()
    {
        var barema = new Barema();

        var resultado = barema.CalcularNotaFinal(null!);

        resultado.Should().Be(0);
    }

    [Fact]
    public void CalcularNotaFinal_RetornaZeroSeVazio()
    {
        var barema = new Barema();

        var resultado = barema.CalcularNotaFinal(new Dictionary<string, float>());

        resultado.Should().Be(0);
    }

    [Fact]
    public void CalcularNotaFinal_CalculaMedia()
    {
        var barema = new Barema();
        var criterios = new Dictionary<string, float>
        {
            { "originalidade", 8 },
            { "relevancia", 6 },
            { "metodologia", 7 },
            { "apresentacao", 9 }
        };

        var resultado = barema.CalcularNotaFinal(criterios);

        resultado.Should().Be(7.5f);
    }

    [Fact]
    public void ValidarCompletude_RetornaTrueSeCompleto()
    {
        var barema = new Barema
        {
            CriteriosJson = "{\"originalidade\":8}",
            DataPreenchimento = DateTime.UtcNow
        };

        barema.ValidarCompletude().Should().BeTrue();
    }

    [Fact]
    public void ValidarCompletude_RetornaFalseSeSemCriterios()
    {
        var barema = new Barema
        {
            DataPreenchimento = DateTime.UtcNow
        };

        barema.ValidarCompletude().Should().BeFalse();
    }

    [Fact]
    public void ValidarCompletude_RetornaFalseSeSemData()
    {
        var barema = new Barema
        {
            CriteriosJson = "{\"originalidade\":8}"
        };

        barema.ValidarCompletude().Should().BeFalse();
    }
}
