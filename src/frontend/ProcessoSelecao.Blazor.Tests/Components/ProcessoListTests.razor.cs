using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ProcessoSelecao.Blazor.Components.Pages.Admin;
using ProcessoSelecao.Blazor.Models;
using ProcessoSelecao.Blazor.Services;

namespace ProcessoSelecao.Blazor.Tests.Components;

public class ProcessoListTests : TestContext
{
    private readonly Mock<ProcessoSelecaoService> _mockService;

    public ProcessoListTests()
    {
        var mockApi = new Mock<ApiService>(new HttpClient(), new Mock<ILogger<ApiService>>().Object);
        _mockService = new Mock<ProcessoSelecaoService>(mockApi.Object);
        Services.AddSingleton(_mockService.Object);
    }

    [Fact]
    public void ShouldRenderTableHeaders()
    {
        _mockService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Models.ProcessoSelecao>());

        var cut = RenderComponent<ProcessoList>();

        cut.Find("h1").TextContent.Should().Contain("Processos de Seleção");
        cut.Find("table").Should().NotBeNull();
        cut.FindAll("th").Count.Should().Be(10);
    }

    [Fact]
    public void ShouldShowEmptyStateWhenNoProcessos()
    {
        _mockService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Models.ProcessoSelecao>());

        var cut = RenderComponent<ProcessoList>();

        cut.FindAll("tbody tr").Count.Should().Be(0);
    }

    [Fact]
    public void ShouldShowNovoProcessoButton()
    {
        _mockService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Models.ProcessoSelecao>());

        var cut = RenderComponent<ProcessoList>();

        cut.Find("button").TextContent.Should().Contain("Novo Processo");
    }
}
