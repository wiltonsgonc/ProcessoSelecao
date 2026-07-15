using FluentAssertions;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;
using ProcessoSelecaoEntity = ProcessoSelecao.Domain.Entities.ProcessoSelecao;

namespace ProcessoSelecao.Tests.Domain;

public class ProcessoSelecaoTests
{
    [Fact]
    public void IniciarProcesso_DeRascunhoParaAberto()
    {
        var processo = new ProcessoSelecaoEntity { Status = StatusProcesso.Rascunho };

        processo.IniciarProcesso();

        processo.Status.Should().Be(StatusProcesso.Aberto);
    }

    [Fact]
    public void IniciarProcesso_NaoAlteraSeNaoForRascunho()
    {
        var processo = new ProcessoSelecaoEntity { Status = StatusProcesso.Aberto };

        processo.IniciarProcesso();

        processo.Status.Should().Be(StatusProcesso.Aberto);
    }

    [Fact]
    public void IniciarProcesso_DefineDataInicioSeDefault()
    {
        var processo = new ProcessoSelecaoEntity { Status = StatusProcesso.Rascunho, DataInicio = default };

        processo.IniciarProcesso();

        processo.DataInicio.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void AbrirAutomaticamente_DeveAbrirSeDataInicioChegou()
    {
        var processo = new ProcessoSelecaoEntity
        {
            Status = StatusProcesso.Rascunho,
            DataInicio = DateTime.UtcNow.AddHours(-1)
        };

        var resultado = processo.AbrirAutomaticamente();

        resultado.Should().BeTrue();
        processo.Status.Should().Be(StatusProcesso.Aberto);
    }

    [Fact]
    public void AbrirAutomaticamente_NaoDeveAbrirSeDataInicioFutura()
    {
        var processo = new ProcessoSelecaoEntity
        {
            Status = StatusProcesso.Rascunho,
            DataInicio = DateTime.UtcNow.AddHours(1)
        };

        var resultado = processo.AbrirAutomaticamente();

        resultado.Should().BeFalse();
        processo.Status.Should().Be(StatusProcesso.Rascunho);
    }

    [Fact]
    public void FinalizarProcesso_DeAbertoParaFinalizado()
    {
        var processo = new ProcessoSelecaoEntity { Status = StatusProcesso.Aberto };

        processo.FinalizarProcesso();

        processo.Status.Should().Be(StatusProcesso.Finalizado);
        processo.DataFim.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void FinalizarProcesso_NaoFinalizaSeRascunho()
    {
        var processo = new ProcessoSelecaoEntity { Status = StatusProcesso.Rascunho };

        processo.FinalizarProcesso();

        processo.Status.Should().Be(StatusProcesso.Rascunho);
    }

    [Fact]
    public void VerificarPrazoExpirado_FinalizaSePrazoPassou()
    {
        var processo = new ProcessoSelecaoEntity
        {
            Status = StatusProcesso.EmAndamento,
            DataFim = DateTime.UtcNow.AddHours(-1)
        };

        var resultado = processo.VerificarPrazoExpirado();

        resultado.Should().BeTrue();
        processo.Status.Should().Be(StatusProcesso.Finalizado);
    }

    [Fact]
    public void VerificarPrazoExpirado_NaoFinalizaSeDentroDoPrazo()
    {
        var processo = new ProcessoSelecaoEntity
        {
            Status = StatusProcesso.EmAndamento,
            DataFim = DateTime.UtcNow.AddHours(1)
        };

        var resultado = processo.VerificarPrazoExpirado();

        resultado.Should().BeFalse();
        processo.Status.Should().Be(StatusProcesso.EmAndamento);
    }

    [Fact]
    public void EstaDentroDoPrazo_RetornaTrueSeDentroDaJanela()
    {
        var processo = new ProcessoSelecaoEntity
        {
            DataInicio = DateTime.UtcNow.AddHours(-1),
            DataFim = DateTime.UtcNow.AddHours(1)
        };

        processo.EstaDentroDoPrazo().Should().BeTrue();
    }

    [Fact]
    public void EstaDentroDoPrazo_RetornaFalseSeAntesDoInicio()
    {
        var processo = new ProcessoSelecaoEntity
        {
            DataInicio = DateTime.UtcNow.AddHours(1),
            DataFim = DateTime.UtcNow.AddHours(2)
        };

        processo.EstaDentroDoPrazo().Should().BeFalse();
    }

    [Fact]
    public void ReverterSePrazoValido_ReverteSeFinalizadoDentroDoPrazo()
    {
        var processo = new ProcessoSelecaoEntity
        {
            Status = StatusProcesso.Finalizado,
            DataInicio = DateTime.UtcNow.AddHours(-2),
            DataFim = DateTime.UtcNow.AddHours(1)
        };

        processo.ReverterSePrazoValido();

        processo.Status.Should().Be(StatusProcesso.EmAndamento);
    }

    [Fact]
    public void ReverterSePrazoValido_NaoReverteSePrazoExpirado()
    {
        var processo = new ProcessoSelecaoEntity
        {
            Status = StatusProcesso.Finalizado,
            DataInicio = DateTime.UtcNow.AddHours(-2),
            DataFim = DateTime.UtcNow.AddHours(-1)
        };

        processo.ReverterSePrazoValido();

        processo.Status.Should().Be(StatusProcesso.Finalizado);
    }
}
