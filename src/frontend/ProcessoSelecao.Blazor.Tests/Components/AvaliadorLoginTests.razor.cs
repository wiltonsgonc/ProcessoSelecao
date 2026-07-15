using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using ProcessoSelecao.Blazor.Components.Pages.Avaliador;
using ProcessoSelecao.Blazor.Models;
using System.Net;
using System.Net.Http.Json;

namespace ProcessoSelecao.Blazor.Tests.Components;

public class AvaliadorLoginTests : TestContext
{
    [Fact]
    public void ShouldRenderLoginForm()
    {
        var cut = RenderComponent<AvaliadorLogin>();

        cut.Find("input[type='text']").Should().NotBeNull();
        cut.Find("input[type='password']").Should().NotBeNull();
        cut.Find("button[type='submit']").Should().NotBeNull();
        cut.Markup.Should().Contain("Área do Avaliador");
    }

    [Fact]
    public void ShouldShowErrorOnInvalidCredentials()
    {
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized));

        var httpClient = new HttpClient(mockHandler.Object);
        Services.AddSingleton(httpClient);

        var cut = RenderComponent<AvaliadorLogin>();

        cut.Find("input[type='text']").Change("12345678901");
        cut.Find("input[type='password']").Change("wrong");
        cut.Find("form").Submit();

        cut.WaitForState(() => cut.Markup.Contains("CPF ou senha inválidos"));
        cut.Find("div.bg-red-50").TextContent.Should().Contain("CPF ou senha inválidos");
    }
}
