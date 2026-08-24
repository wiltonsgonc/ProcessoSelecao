using Microsoft.AspNetCore.Mvc;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Application.Services;

namespace ProcessoSelecao.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CandidatoAuthController : ControllerBase
{
    private readonly ICandidatoAuthService _authService;

    public CandidatoAuthController(ICandidatoAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("verificar")]
    public async Task<ActionResult<CandidatoAuthResponseDto>> Verificar([FromBody] VerificarCandidatoDto dto)
    {
        var result = await _authService.VerificarCandidatoAsync(dto.Cpf, dto.Email, dto.DataNascimento);
        if (result == null)
            return NotFound(new { message = "Candidato não encontrado. Realize a inscrição." });
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<CandidatoAuthResponseDto>> Login([FromBody] LoginCandidatoDto dto)
    {
        var result = await _authService.LoginAsync(dto.Cpf, dto.Email, dto.DataNascimento, dto.Senha);
        if (result == null)
            return Unauthorized(new { message = "Credenciais inválidas ou senha não definida." });
        return Ok(result);
    }

    [HttpPost("definir-senha")]
    public async Task<IActionResult> DefinirSenha([FromBody] DefinirSenhaCandidatoDto dto)
    {
        await _authService.DefinirSenhaAsync(dto.CandidatoId, dto.Senha);
        return Ok(new { message = "Senha definida com sucesso." });
    }
}
