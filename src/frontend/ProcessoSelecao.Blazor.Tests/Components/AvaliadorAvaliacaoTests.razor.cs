using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using ProcessoSelecao.Blazor.Components.Pages.Avaliador;
using ProcessoSelecao.Blazor.Models;

namespace ProcessoSelecao.Blazor.Tests.Components;

public class AvaliadorAvaliacaoTests : TestContext
{
    [Fact]
    public void ShouldNotRedirectInDevelopmentMode()
    {
        var mockJS = new Mock<IJSRuntime>();
        mockJS.Setup(x => x.InvokeAsync<string>("sessionStorage.getItem", It.IsAny<object[]>()))
            .ReturnsAsync("");
        Services.AddSingleton(mockJS.Object);

        var mockEnv = new Mock<IWebHostEnvironment>();
        mockEnv.Setup(e => e.EnvironmentName).Returns("Development");
        Services.AddSingleton(mockEnv.Object);

        var navMan = Services.GetRequiredService<NavigationManager>();

        RenderComponent<AvaliadorAvaliacao>(parameters => parameters.Add(p => p.baremaId, 1));

        navMan.Uri.Should().NotContain("/avaliador/login");
    }
}
