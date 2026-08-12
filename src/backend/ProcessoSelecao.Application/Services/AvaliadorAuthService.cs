using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Domain.Helpers;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Application.Services;

public interface IAvaliadorAuthService
{
    Task<AvaliadorAuthResponseDto?> LoginAsync(string cpf, string senha);
    Task DefinirSenhaAsync(long avaliadorId, string senha);
}

public class AvaliadorAuthService : IAvaliadorAuthService
{
    private readonly IAvaliadorRepository _repository;
    private readonly IConfiguration _configuration;

    public AvaliadorAuthService(IAvaliadorRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<AvaliadorAuthResponseDto?> LoginAsync(string cpf, string senha)
    {
        var cpfLimpo = CpfValidator.Clean(cpf);
        var avaliador = await _repository.GetByCpfAsync(cpfLimpo);
        if (avaliador == null || !avaliador.Ativo)
            return null;

        if (string.IsNullOrEmpty(avaliador.SenhaHash))
            return null;

        if (!BCrypt.Net.BCrypt.Verify(senha, avaliador.SenhaHash))
            return null;

        var expiracao = DateTime.UtcNow.AddHours(8);
        var token = GerarToken(avaliador.Id, avaliador.Nome, avaliador.Email, expiracao);

        return new AvaliadorAuthResponseDto
        {
            Token = token,
            AvaliadorId = avaliador.Id,
            Nome = avaliador.Nome,
            Email = avaliador.Email,
            Expiracao = expiracao
        };
    }

    public async Task DefinirSenhaAsync(long avaliadorId, string senha)
    {
        var avaliador = await _repository.GetByIdAsync(avaliadorId)
            ?? throw new Exception("Avaliador não encontrado");

        avaliador.SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha);
        avaliador.DataAtualizacao = DateTime.UtcNow;
        await _repository.UpdateAsync(avaliador);
    }

    private string GerarToken(long avaliadorId, string nome, string email, DateTime expiracao)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"]
            ?? _configuration["JwtSettings__SecretKey"]
            ?? "ProcessoSelecao_SecretKey_Minimo32Caracteres_2026!";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("avaliadorId", avaliadorId.ToString()),
            new Claim("nome", nome),
            new Claim("email", email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"] ?? "ProcessoSelecaoApi",
            audience: _configuration["JwtSettings:Audience"] ?? "ProcessoSelecaoWeb",
            claims: claims,
            expires: expiracao,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
