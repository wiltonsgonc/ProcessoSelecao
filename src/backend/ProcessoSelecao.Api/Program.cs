using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using ProcessoSelecao.Application;
using ProcessoSelecao.Application.Services;
using ProcessoSelecao.Domain.Interfaces;
using ProcessoSelecao.Infrastructure.Data;
using ProcessoSelecao.Infrastructure.Repositories;

/// <summary>
/// Configuração e inicialização da aplicação ASP.NET Core
/// </summary>
// Carrega variáveis do .env (gitignored) ANTES de criar o builder,
// para que ConnectionStrings__DefaultConnection, JwtSettings, etc. sejam
// lidos pela configuração (AddEnvironmentVariables ocorre dentro de CreateBuilder).
LoadDotEnv();

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;

// Configura a URL da aplicação
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // API e Swagger na porta 5002
    serverOptions.ListenAnyIP(5002);
    // Aplicação web na porta 5000
    serverOptions.ListenAnyIP(5000);
});

// Adiciona serviços de controllers e API Explorer
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Adiciona Swagger/OpenAPI
builder.Services.AddSwaggerGen();

// Configuração do Entity Framework Core com SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.MigrationsAssembly("ProcessoSelecao.Infrastructure")));

// ============================================
// Registro de Repositories (Dependency Injection)
// ============================================
builder.Services.AddScoped<ICandidatoRepository, CandidatoRepository>();
builder.Services.AddScoped<IDocumentoRepository, DocumentoRepository>();
builder.Services.AddScoped<IAvaliadorRepository, AvaliadorRepository>();
builder.Services.AddScoped<IBaremaRepository, BaremaRepository>();
builder.Services.AddScoped<IProcessoSelecaoRepository, ProcessoSelecaoRepository>();

// ============================================
// Registro de Services
// ============================================
builder.Services.AddScoped<ICandidatoService, CandidatoService>();
builder.Services.AddScoped<IDocumentoService, DocumentoService>();
builder.Services.AddScoped<IAvaliadorService, AvaliadorService>();
builder.Services.AddScoped<IBaremaService, BaremaService>();
builder.Services.AddScoped<IProcessoSelecaoService, ProcessoSelecaoService>();
builder.Services.AddScoped<IEmailNotificationService, EmailNotificationService>();
builder.Services.AddScoped<IInscricaoService, InscricaoService>();

// ============================================
// Configuração de Email
// ============================================
builder.Services.AddSingleton<EmailSettings>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new EmailSettings
    {
        SmtpHost = config["EmailSettings:SmtpHost"] ?? "",
        SmtpPort = int.Parse(config["EmailSettings:SmtpPort"] ?? "587"),
        SmtpUser = config["EmailSettings:SmtpUser"] ?? "",
        SmtpPassword = config["EmailSettings:SmtpPassword"] ?? "",
        FromEmail = config["EmailSettings:FromEmail"] ?? ""
    };
});

// Configuração do AutoMapper (chave vazia = OSS license, sem bloqueio)
builder.Services.AddAutoMapper(cfg => cfg.LicenseKey = "", typeof(MappingProfile));
builder.Logging.AddFilter("LuckyPennySoftware.AutoMapper.License", LogLevel.None);

// Configuração de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "http://frontend:4200",
                "http://localhost:5119",
                "http://localhost:7209",
                "http://10.255.255.254:5119")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ============================================
// Aplicar migrations automaticamente
// ============================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// ============================================
// Configuração do Pipeline de Requisições
// ============================================

// Swagger em ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Endpoint de health check
app.MapGet("/api/health", () => "OK");

// CORS
app.UseCors("AllowFrontend");

// Autorização
app.UseAuthorization();

// Mapeamento dos controllers
app.MapControllers();

app.Run();

// ============================================
// Helpers
// ============================================

// Procura o arquivo .env subindo a partir do diretório da aplicação
// (bin/Debug/netX.0 → ... → raiz do repositório) e o carrega.
static void LoadDotEnv()
{
    var dir = new DirectoryInfo(AppContext.BaseDirectory);
    while (dir is not null)
    {
        var candidate = Path.Combine(dir.FullName, ".env");
        if (File.Exists(candidate))
        {
            DotNetEnv.Env.Load(candidate);
            return;
        }
        dir = dir.Parent;
    }
}
