using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Domain.Helpers;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Application.Services;

public interface ICandidatoAuthService
{
    Task<CandidatoAuthResponseDto?> VerificarCandidatoAsync(string? cpf, string? email, DateTime? dataNascimento);
    Task<CandidatoAuthResponseDto?> LoginAsync(string? cpf, string? email, DateTime? dataNascimento, string senha);
    Task DefinirSenhaAsync(long candidatoId, string senha);
}

public class CandidatoAuthService : ICandidatoAuthService
{
    private readonly ICandidatoRepository _repository;
    private readonly IConfiguration _configuration;

    public CandidatoAuthService(ICandidatoRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<CandidatoAuthResponseDto?> VerificarCandidatoAsync(string? cpf, string? email, DateTime? dataNascimento)
    {
        Domain.Entities.Candidato? candidato = null;

        if (!string.IsNullOrWhiteSpace(cpf))
        {
            var cpfLimpo = CpfValidator.Clean(cpf);
            candidato = await _repository.GetByCpfAsync(cpfLimpo);
        }
        else if (!string.IsNullOrWhiteSpace(email) && dataNascimento.HasValue)
        {
            candidato = await _repository.GetByEmailAndDataNascimentoAsync(email, dataNascimento.Value);
        }

        if (candidato == null)
            return null;

        var primeiroAcesso = string.IsNullOrEmpty(candidato.SenhaHash);

        return new CandidatoAuthResponseDto
        {
            CandidatoId = candidato.Id,
            Nome = candidato.Nome,
            Email = candidato.Email,
            PrimeiroAcesso = primeiroAcesso
        };
    }

    public async Task<CandidatoAuthResponseDto?> LoginAsync(string? cpf, string? email, DateTime? dataNascimento, string senha)
    {
        Domain.Entities.Candidato? candidato = null;

        if (!string.IsNullOrWhiteSpace(cpf))
        {
            var cpfLimpo = CpfValidator.Clean(cpf);
            candidato = await _repository.GetByCpfAsync(cpfLimpo);
        }
        else if (!string.IsNullOrWhiteSpace(email) && dataNascimento.HasValue)
        {
            candidato = await _repository.GetByEmailAndDataNascimentoAsync(email, dataNascimento.Value);
        }

        if (candidato == null)
            return null;

        if (string.IsNullOrEmpty(candidato.SenhaHash))
            return null;

        if (!BCrypt.Net.BCrypt.Verify(senha, candidato.SenhaHash))
            return null;

        var expiracao = DateTime.UtcNow.AddHours(8);
        var token = GerarToken(candidato.Id, candidato.Nome, candidato.Email, expiracao);

        return new CandidatoAuthResponseDto
        {
            Token = token,
            CandidatoId = candidato.Id,
            Nome = candidato.Nome,
            Email = candidato.Email,
            Expiracao = expiracao,
            PrimeiroAcesso = false
        };
    }

    public async Task DefinirSenhaAsync(long candidatoId, string senha)
    {
        var candidato = await _repository.GetByIdAsync(candidatoId)
            ?? throw new Exception("Candidato não encontrado");

        candidato.SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha);
        candidato.DataCadastro = candidato.DataCadastro;
        await _repository.UpdateAsync(candidato);
    }

    private string GerarToken(long candidatoId, string nome, string email, DateTime expiracao)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"]
            ?? _configuration["JwtSettings__SecretKey"]
            ?? "ProcessoSelecao_SecretKey_Minimo32Caracteres_2026!";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("candidatoId", candidatoId.ToString()),
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
