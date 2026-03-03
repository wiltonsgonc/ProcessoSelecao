using Microsoft.EntityFrameworkCore;
using ProcessoSelecao.Application;
using ProcessoSelecao.Application.Services;
using ProcessoSelecao.Domain.Interfaces;
using ProcessoSelecao.Infrastructure.Data;
using ProcessoSelecao.Infrastructure.Repositories;

/// <summary>
/// Configuração e inicialização da aplicação ASP.NET Core
/// </summary>
var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;

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

// Configuração do AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configuração de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://frontend:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ============================================
// Configuração do Pipeline de Requisições
// ============================================

// Swagger em ambiente de desenvolvimento
app.UseSwagger();
app.UseSwaggerUI();

// Endpoint de health check
app.MapGet("/api/health", () => "OK");

// CORS
app.UseCors("AllowAngular");

// Autorização
app.UseAuthorization();

// Mapeamento dos controllers
app.MapControllers();

app.Run();
