using FluentAssertions;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Tests.Domain;

public class AvaliadorTests
{
    [Fact]
    public void ListarAvaliacoesPendentes_RetornaPendentesEEmPreenchimento()
    {
        var avaliador = new Avaliador
        {
            Baremas = new List<Barema>
            {
                new() { Status = StatusBarema.Pendente },
                new() { Status = StatusBarema.EmPreenchimento },
                new() { Status = StatusBarema.Concluido },
                new() { Status = StatusBarema.Pendente }
            }
        };

        var resultado = avaliador.ListarAvaliacoesPendentes();

        resultado.Should().HaveCount(3);
        resultado.Should().OnlyContain(b => b.Status == StatusBarema.Pendente || b.Status == StatusBarema.EmPreenchimento);
    }

    [Fact]
    public void ListarAvaliacoesPendentes_RetornaVazioSeNaoTemNenhuma()
    {
        var avaliador = new Avaliador
        {
            Baremas = new List<Barema>
            {
                new() { Status = StatusBarema.Concluido },
                new() { Status = StatusBarema.Cancelado }
            }
        };

        var resultado = avaliador.ListarAvaliacoesPendentes();

        resultado.Should().BeEmpty();
    }
}
