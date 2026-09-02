using FluentAssertions;
using ProcessoSelecao.Domain.Entities;

namespace ProcessoSelecao.Tests.Domain;

public class BaremaPibicTests
{
    [Fact]
    public void CalcularNotaFinalPibic_RetornaZeroSeJsonVazio()
    {
        var barema = new Barema();

        var resultado = barema.CalcularNotaFinalPibic("");

        resultado.Should().Be(0);
    }

    [Fact]
    public void CalcularNotaFinalPibic_RetornaZeroSeJsonNull()
    {
        var barema = new Barema();

        var resultado = barema.CalcularNotaFinalPibic(null!);

        resultado.Should().Be(0);
    }

    [Fact]
    public void CalcularNotaFinalPibic_LancaExcecao_SeJsonInvalido()
    {
        var barema = new Barema();

        var act = () => barema.CalcularNotaFinalPibic("invalido");

        act.Should().Throw<System.Text.Json.JsonException>();
    }

    [Fact]
    public void CalcularNotaFinalPibic_SomaSecoesCorretamente()
    {
        var barema = new Barema();
        var json = """
        {
            "projeto": { "criterio1": 10, "criterio2": 15 },
            "orientador": { "criterio3": 10, "criterio4": 10 },
            "candidato": { "criterio5": 15, "criterio6": 15 }
        }
        """;

        var resultado = barema.CalcularNotaFinalPibic(json);

        resultado.Should().Be(75f);
    }

    [Fact]
    public void CalcularNotaFinalPibic_SomaParcialSeSecoesFaltando()
    {
        var barema = new Barema();
        var json = """
        {
            "projeto": { "criterio1": 10, "criterio2": 15 }
        }
        """;

        var resultado = barema.CalcularNotaFinalPibic(json);

        resultado.Should().Be(25f);
    }

    [Fact]
    public void CalcularNotaFinalPibic_LidaComJsonElementNulo()
    {
        var barema = new Barema();
        var json = """
        {
            "projeto": null,
            "orientador": null,
            "candidato": null
        }
        """;

        var resultado = barema.CalcularNotaFinalPibic(json);

        resultado.Should().Be(0f);
    }

    [Fact]
    public void CalcularNotaFinalPibic_MenorQue100_QuandoDadosCompletos()
    {
        var barema = new Barema();
        var json = """
        {
            "projeto": { "item1": 35 },
            "orientador": { "item2": 35 },
            "candidato": { "item3": 30 }
        }
        """;

        var resultado = barema.CalcularNotaFinalPibic(json);

        resultado.Should().Be(100f);
    }
}
