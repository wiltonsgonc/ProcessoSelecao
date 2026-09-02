using FluentAssertions;
using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Tests.Domain;

public class TipoProcessoTests
{
    [Fact]
    public void TipoProcesso_ContemValoresEsperados()
    {
        var valores = Enum.GetValues(typeof(TipoProcesso)).Cast<TipoProcesso>().ToList();

        valores.Should().Contain(TipoProcesso.PIBIC);
        valores.Should().Contain(TipoProcesso.PIBIT);
        valores.Should().Contain(TipoProcesso.PRH27);
        valores.Should().Contain(TipoProcesso.MCTI);
        valores.Should().Contain(TipoProcesso.GETEC);
        valores.Should().Contain(TipoProcesso.MPDS);
    }

    [Fact]
    public void TipoProcesso_Tem6Valores()
    {
        var valores = Enum.GetValues(typeof(TipoProcesso));

        valores.Length.Should().Be(6);
    }

    [Theory]
    [InlineData(TipoProcesso.PIBIC, 0)]
    [InlineData(TipoProcesso.PIBIT, 1)]
    [InlineData(TipoProcesso.PRH27, 2)]
    [InlineData(TipoProcesso.MCTI, 3)]
    [InlineData(TipoProcesso.GETEC, 4)]
    [InlineData(TipoProcesso.MPDS, 5)]
    public void TipoProcesso_ValoresInteirosCorretos(TipoProcesso tipo, int esperado)
    {
        ((int)tipo).Should().Be(esperado);
    }
}
