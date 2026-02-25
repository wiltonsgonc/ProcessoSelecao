using Microsoft.EntityFrameworkCore;
using ProcessoSelecao.Application;
using ProcessoSelecao.Application.Services;
using ProcessoSelecao.Domain.Interfaces;
using ProcessoSelecao.Infrastructure.Data;
using ProcessoSelecao.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

if (environment == "Development")
{
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "ProcessoSelecao API", Version = "v1" });
    });
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICandidatoRepository, CandidatoRepository>();
builder.Services.AddScoped<IDocumentoRepository, DocumentoRepository>();
builder.Services.AddScoped<IAvaliadorRepository, AvaliadorRepository>();
builder.Services.AddScoped<IBaremaRepository, BaremaRepository>();
builder.Services.AddScoped<IProcessoSelecaoRepository, ProcessoSelecaoRepository>();

builder.Services.AddScoped<ICandidatoService, CandidatoService>();
builder.Services.AddScoped<IDocumentoService, DocumentoService>();
builder.Services.AddScoped<IAvaliadorService, AvaliadorService>();
builder.Services.AddScoped<IBaremaService, BaremaService>();
builder.Services.AddScoped<IProcessoSelecaoService, ProcessoSelecaoService>();
builder.Services.AddScoped<IEmailNotificationService, EmailNotificationService>();

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

builder.Services.AddAutoMapper(typeof(MappingProfile));

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

if (environment == "Development")
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProcessoSelecao API v1");
    });
}

app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();
app.Run();
