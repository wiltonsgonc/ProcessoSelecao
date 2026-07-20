using ProcessoSelecao.Blazor.Components;
using ProcessoSelecao.Blazor.Services;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;

// Carrega appsettings.WSL.json quando rodando no WSL
// Uso: ASPNETCORE_ENVIRONMENT=WSL dotnet watch --project ProcessoSelecao.Blazor
if (env.EnvironmentName == "WSL")
{
    builder.Configuration.AddJsonFile("appsettings.WSL.json", optional: true, reloadOnChange: true);
}

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri(
        (builder.Configuration["Backend:BaseUrl"] ?? "http://localhost:5002/api").TrimEnd('/') + "/");
});

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<ProcessoSelecaoService>();
builder.Services.AddScoped<CandidatoService>();
builder.Services.AddScoped<AvaliadorService>();
builder.Services.AddScoped<DocumentoService>();
builder.Services.AddScoped<BaremaService>();
builder.Services.AddScoped<FormularioService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment() && app.Environment.EnvironmentName != "WSL")
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
