using Microsoft.AspNetCore.Mvc;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Application.Services;

namespace ProcessoSelecao.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AvaliadorAuthController : ControllerBase
{
    private readonly IAvaliadorAuthService _authService;

    public AvaliadorAuthController(IAvaliadorAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AvaliadorAuthResponseDto>> Login([FromBody] LoginAvaliadorDto dto)
    {
        var result = await _authService.LoginAsync(dto.Cpf, dto.Senha);
        if (result == null)
            return Unauthorized("CPF ou senha inválidos.");
        return Ok(result);
    }

    [HttpPost("definir-senha")]
    public async Task<IActionResult> DefinirSenha([FromBody] DefinirSenhaDto dto)
    {
        await _authService.DefinirSenhaAsync(dto.AvaliadorId, dto.Senha);
        return Ok(new { message = "Senha definida com sucesso." });
    }
}
