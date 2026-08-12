using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;

namespace ProcessoSelecao.Blazor.Tests.Helpers;

public static class ServiceMockExtensions
{
    public static Mock<IJSRuntime> AsJSRuntime(this TestContext context)
    {
        var mock = new Mock<IJSRuntime>();
        context.Services.AddSingleton(mock.Object);
        return mock;
    }
}
