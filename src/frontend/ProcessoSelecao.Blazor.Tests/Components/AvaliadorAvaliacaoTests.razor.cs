using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using ProcessoSelecao.Blazor.Components.Pages.Avaliador;
using ProcessoSelecao.Blazor.Models;

namespace ProcessoSelecao.Blazor.Tests.Components;

public class AvaliadorAvaliacaoTests : TestContext
{
    [Fact]
    public void ShouldRedirectToLoginWhenNoToken()
    {
        var mockJS = new Mock<IJSRuntime>();
        mockJS.Setup(x => x.InvokeAsync<string>("sessionStorage.getItem", It.IsAny<object[]>()))
            .ReturnsAsync("");
        Services.AddSingleton(mockJS.Object);

        var navMan = Services.GetRequiredService<NavigationManager>();

        RenderComponent<AvaliadorAvaliacao>(parameters => parameters.Add(p => p.baremaId, 1));

        navMan.Uri.Should().Contain("/avaliador/login");
    }
}
