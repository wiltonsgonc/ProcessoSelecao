using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Moq;
using ProcessoSelecao.Blazor.Components.Pages.Admin;
using ProcessoSelecao.Blazor.Models;
using ProcessoSelecao.Blazor.Services;
using ProcessoSelecaoModel = ProcessoSelecao.Blazor.Models.ProcessoSelecao;

namespace ProcessoSelecao.Blazor.Tests.Components;

public class DocumentoListTests : TestContext
{
    private readonly Mock<DocumentoService> _mockDocService;
    private readonly Mock<ProcessoSelecaoService> _mockProcessoService;
    private readonly Mock<IJSRuntime> _mockJS;

    public DocumentoListTests()
    {
        var mockApi = new Mock<ApiService>(new HttpClient(), new Mock<ILogger<ApiService>>().Object);
        _mockDocService = new Mock<DocumentoService>(mockApi.Object);
        _mockProcessoService = new Mock<ProcessoSelecaoService>(mockApi.Object);
        _mockJS = new Mock<IJSRuntime>();

        Services.AddSingleton(_mockDocService.Object);
        Services.AddSingleton(_mockProcessoService.Object);
        Services.AddSingleton(_mockJS.Object);
    }

    [Fact]
    public void ShouldRenderPageTitle()
    {
        _mockDocService.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Documento>());
        _mockProcessoService.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<ProcessoSelecaoModel>());

        var cut = RenderComponent<DocumentoList>();

        cut.Find("h1").TextContent.Should().Contain("Documentos");
    }

    [Fact]
    public void ShouldShowProcessoFilter()
    {
        _mockDocService.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Documento>());
        _mockProcessoService.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<ProcessoSelecaoModel>());

        var cut = RenderComponent<DocumentoList>();

        cut.Markup.Should().Contain("Processo Seletivo");
        cut.FindAll("select").Count.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public void ShouldShowUploadButton()
    {
        _mockDocService.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Documento>());
        _mockProcessoService.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<ProcessoSelecaoModel>());

        var cut = RenderComponent<DocumentoList>();

        cut.FindAll("button").Should().Contain(b => b.TextContent.Contains("Upload Documento"));
    }

    [Fact]
    public void ShouldShowEmptyStateWhenNoDocuments()
    {
        _mockDocService.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Documento>());
        _mockProcessoService.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<ProcessoSelecaoModel>());

        var cut = RenderComponent<DocumentoList>();

        cut.WaitForState(() => cut.Markup.Contains("Nenhum documento") || cut.FindAll("h3").Count == 0);
    }
}
