using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Moq;
using ProcessoSelecao.Blazor.Components.Pages.Admin;
using ProcessoSelecao.Blazor.Services;
using ProcessoSelecaoModel = ProcessoSelecao.Blazor.Models.ProcessoSelecao;

namespace ProcessoSelecao.Blazor.Tests.Components;

public class DefinirAvaliadorTests : TestContext
{
    private readonly Mock<ProcessoSelecaoService> _mockProcessoService;
    private readonly Mock<CandidatoService> _mockCandidatoService;
    private readonly Mock<AvaliadorService> _mockAvaliadorService;
    private readonly Mock<BaremaService> _mockBaremaService;
    private readonly Mock<IJSRuntime> _mockJS;

    public DefinirAvaliadorTests()
    {
        var mockApi = new Mock<ApiService>(new HttpClient(), new Mock<ILogger<ApiService>>().Object);
        _mockProcessoService = new Mock<ProcessoSelecaoService>(mockApi.Object);
        _mockCandidatoService = new Mock<CandidatoService>(mockApi.Object);
        _mockAvaliadorService = new Mock<AvaliadorService>(mockApi.Object);
        _mockBaremaService = new Mock<BaremaService>(mockApi.Object);
        _mockJS = new Mock<IJSRuntime>();

        Services.AddSingleton(_mockProcessoService.Object);
        Services.AddSingleton(_mockCandidatoService.Object);
        Services.AddSingleton(_mockAvaliadorService.Object);
        Services.AddSingleton(_mockBaremaService.Object);
        Services.AddSingleton(_mockJS.Object);
    }

    [Fact]
    public void ShouldRenderPageTitle()
    {
        _mockProcessoService.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<ProcessoSelecaoModel>());

        var cut = RenderComponent<DefinirAvaliador>();

        cut.Find("h1").TextContent.Should().Contain("Definir Avaliadores");
    }

    [Fact]
    public void ShouldShowLimparButton()
    {
        _mockProcessoService.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<ProcessoSelecaoModel>());

        var cut = RenderComponent<DefinirAvaliador>();

        cut.FindAll("button").Should().Contain(b => b.TextContent.Contains("Limpar"));
    }

    [Fact]
    public void ShouldShowStep1ProcessoDropdown()
    {
        _mockProcessoService.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<ProcessoSelecaoModel>());

        var cut = RenderComponent<DefinirAvaliador>();

        cut.Markup.Should().Contain("Selecionar Processo");
        cut.Find("select").Should().NotBeNull();
    }

    [Fact]
    public void ShouldDisableConfirmButtonWhenNoEvaluators()
    {
        _mockProcessoService.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<ProcessoSelecaoModel>());

        var cut = RenderComponent<DefinirAvaliador>();

        var confirmButton = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("Criar"));
        confirmButton.Should().NotBeNull();
        confirmButton!.HasAttribute("disabled").Should().BeTrue();
    }
}
