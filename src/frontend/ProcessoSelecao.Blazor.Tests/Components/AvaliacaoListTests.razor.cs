using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ProcessoSelecao.Blazor.Components.Pages.Admin;
using ProcessoSelecao.Blazor.Models;
using ProcessoSelecao.Blazor.Services;

namespace ProcessoSelecao.Blazor.Tests.Components;

public class AvaliacaoListTests : TestContext
{
    private readonly Mock<BaremaService> _mockBaremaService;

    public AvaliacaoListTests()
    {
        var mockApi = new Mock<ApiService>(new HttpClient(), new Mock<ILogger<ApiService>>().Object);
        _mockBaremaService = new Mock<BaremaService>(mockApi.Object);
        Services.AddSingleton(_mockBaremaService.Object);
    }

    [Fact]
    public void ShouldRenderPageTitle()
    {
        _mockBaremaService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Barema>());

        var cut = RenderComponent<AvaliacaoList>();

        cut.Find("h1").TextContent.Should().Contain("Avaliações");
    }

    [Fact]
    public void ShouldShowFilterDropdown()
    {
        _mockBaremaService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Barema>());

        var cut = RenderComponent<AvaliacaoList>();

        cut.Find("select").Should().NotBeNull();
        cut.FindAll("option").Count.Should().Be(5);
    }

    [Fact]
    public void ShouldShowEmptyStateWhenNoAvaliacoes()
    {
        _mockBaremaService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Barema>());

        var cut = RenderComponent<AvaliacaoList>();

        cut.WaitForState(() => cut.Markup.Contains("Nenhuma avaliação encontrada"));
        cut.Markup.Should().Contain("Nenhuma avaliação encontrada");
    }

    [Fact]
    public void ShouldShowTableWhenBaremasExist()
    {
        var baremas = new List<Barema>
        {
            new()
            {
                Id = 1,
                CandidatoId = 10,
                CandidatoNome = "João Silva",
                AvaliadorId = 20,
                AvaliadorNome = "Maria Santos",
                Status = StatusBarema.Concluido,
                NotaFinal = 8.5,
                Criterios = new Dictionary<string, double>
                {
                    { "originalidade", 8.0 },
                    { "relevancia", 9.0 },
                    { "metodologia", 8.5 },
                    { "apresentacao", 8.5 }
                },
                DataPreenchimento = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            }
        };

        _mockBaremaService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(baremas);

        var cut = RenderComponent<AvaliacaoList>();

        cut.WaitForState(() => cut.FindAll("tbody tr").Count > 0);
        cut.FindAll("tbody tr").Count.Should().Be(1);
        cut.Markup.Should().Contain("João Silva");
    }

    [Fact]
    public void ShouldShowSummaryStatsWhenConcluidasExist()
    {
        var baremas = new List<Barema>
        {
            new() { Id = 1, Status = StatusBarema.Concluido, NotaFinal = 8.0 },
            new() { Id = 2, Status = StatusBarema.Pendente, NotaFinal = 0 },
            new() { Id = 3, Status = StatusBarema.Concluido, NotaFinal = 9.0 }
        };

        _mockBaremaService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(baremas);

        var cut = RenderComponent<AvaliacaoList>();

        cut.WaitForState(() => cut.Markup.Contains("Resumo Geral"));
        cut.Markup.Should().Contain("Resumo Geral");
        cut.Markup.Should().Contain("Concluídas");
        cut.Markup.Should().Contain("Pendentes");
    }
}
