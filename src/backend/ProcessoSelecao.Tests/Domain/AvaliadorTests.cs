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

    [Fact]
    public void NivelCnpq_Default_DeveSerNaoSeAplica()
    {
        var avaliador = new Avaliador();

        avaliador.NivelCnpq.Should().Be(NivelCnpq.NaoSeAplica);
    }

    [Fact]
    public void CamposAcademicos_DevePermitirValoresNulos()
    {
        var avaliador = new Avaliador
        {
            LinkLattes = null,
            UltimaFormacao = null,
            Cargo = null
        };

        avaliador.LinkLattes.Should().BeNull();
        avaliador.UltimaFormacao.Should().BeNull();
        avaliador.Cargo.Should().BeNull();
    }

    [Fact]
    public void CamposAcademicos_DeveArmazenarValoresCorretamente()
    {
        var avaliador = new Avaliador
        {
            LinkLattes = "https://lattes.cnpq.br/1234567890",
            UltimaFormacao = "Doutorado em Ciência da Computação",
            Cargo = "Professora Associada",
            NivelCnpq = NivelCnpq.Pq1D
        };

        avaliador.LinkLattes.Should().Be("https://lattes.cnpq.br/1234567890");
        avaliador.UltimaFormacao.Should().Be("Doutorado em Ciência da Computação");
        avaliador.Cargo.Should().Be("Professora Associada");
        avaliador.NivelCnpq.Should().Be(NivelCnpq.Pq1D);
    }

    [Theory]
    [InlineData(NivelCnpq.NaoSeAplica, 0)]
    [InlineData(NivelCnpq.Pq2, 1)]
    [InlineData(NivelCnpq.Pq1D, 2)]
    [InlineData(NivelCnpq.Pq1C, 3)]
    [InlineData(NivelCnpq.Pq1B, 4)]
    [InlineData(NivelCnpq.Pq1A, 5)]
    [InlineData(NivelCnpq.Dt2, 6)]
    [InlineData(NivelCnpq.Dt1, 7)]
    public void NivelCnpq_DeveTerValoresCorretos(NivelCnpq nivel, int valorEsperado)
    {
        ((int)nivel).Should().Be(valorEsperado);
    }
}
