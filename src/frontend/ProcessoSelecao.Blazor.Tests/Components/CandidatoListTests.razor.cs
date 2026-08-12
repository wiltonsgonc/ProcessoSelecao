using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ProcessoSelecao.Blazor.Components.Pages.Admin;
using ProcessoSelecao.Blazor.Models;
using ProcessoSelecao.Blazor.Services;

namespace ProcessoSelecao.Blazor.Tests.Components;

public class CandidatoListTests : TestContext
{
    private readonly Mock<CandidatoService> _mockCandidatoService;
    private readonly Mock<DocumentoService> _mockDocumentoService;

    public CandidatoListTests()
    {
        var mockApi = new Mock<ApiService>(new HttpClient(), new Mock<ILogger<ApiService>>().Object);
        _mockCandidatoService = new Mock<CandidatoService>(mockApi.Object);
        _mockDocumentoService = new Mock<DocumentoService>(mockApi.Object);
        Services.AddSingleton(_mockCandidatoService.Object);
        Services.AddSingleton(_mockDocumentoService.Object);
    }

    [Fact]
    public void ShouldRenderPageTitle()
    {
        _mockCandidatoService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Candidato>());

        var cut = RenderComponent<CandidatoList>();

        cut.Find("h1").TextContent.Should().Contain("Candidatos");
    }

    [Fact]
    public void ShouldShowNovoCandidatoButton()
    {
        _mockCandidatoService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Candidato>());

        var cut = RenderComponent<CandidatoList>();

        cut.Find("button").TextContent.Should().Contain("Novo Candidato");
    }

    [Fact]
    public void ShouldRenderTableWithAllColumns()
    {
        _mockCandidatoService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Candidato>());

        var cut = RenderComponent<CandidatoList>();

        cut.Find("table").Should().NotBeNull();
        cut.FindAll("th").Count.Should().Be(11);
    }
}
