using FluentAssertions;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Tests.Domain;

public class CandidatoTests
{
    [Fact]
    public void ValidarDocumentos_RetornaFalseSeNaoTemDocumentos()
    {
        var candidato = new Candidato { Documentos = new List<Documento>() };

        candidato.ValidarDocumentos().Should().BeFalse();
    }

    [Fact]
    public void ValidarDocumentos_RetornaTrueSeTodosValidados()
    {
        var candidato = new Candidato
        {
            Documentos = new List<Documento>
            {
                new() { Validado = true },
                new() { Validado = true },
                new() { Validado = true }
            }
        };

        candidato.ValidarDocumentos().Should().BeTrue();
    }

    [Fact]
    public void ValidarDocumentos_RetornaFalseSeAlguemNaoValidado()
    {
        var candidato = new Candidato
        {
            Documentos = new List<Documento>
            {
                new() { Validado = true },
                new() { Validado = false },
                new() { Validado = true }
            }
        };

        candidato.ValidarDocumentos().Should().BeFalse();
    }

    [Fact]
    public void CalcularPontuacao_RetornaZeroSeNaoTemBaremas()
    {
        var candidato = new Candidato { Baremas = new List<Barema>() };

        candidato.CalcularPontuacao().Should().Be(0);
    }

    [Fact]
    public void CalcularPontuacao_CalculaMediaDosConcluidos()
    {
        var candidato = new Candidato
        {
            Baremas = new List<Barema>
            {
                new() { Status = StatusBarema.Concluido, NotaFinal = 8 },
                new() { Status = StatusBarema.Concluido, NotaFinal = 6 },
                new() { Status = StatusBarema.Pendente, NotaFinal = 10 }
            }
        };

        var resultado = candidato.CalcularPontuacao();

        resultado.Should().Be(7f);
    }
}
