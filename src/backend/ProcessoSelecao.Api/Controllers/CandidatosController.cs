using Microsoft.AspNetCore.Mvc;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Application.Services;

namespace ProcessoSelecao.Api.Controllers;

/// <summary>
/// Controller para manipulação de Candidatos
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CandidatosController : ControllerBase
{
    private readonly ICandidatoService _service;

    public CandidatosController(ICandidatoService service)
    {
        _service = service;
    }

    /// <summary>Retorna todos os candidatos</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CandidatoDto>>> GetAll()
    {
        var candidatos = await _service.GetAllAsync();
        return Ok(candidatos);
    }

    /// <summary>Retorna um candidato pelo ID</summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CandidatoDto>> GetById(long id)
    {
        var candidato = await _service.GetByIdAsync(id);
        if (candidato == null) return NotFound();
        return Ok(candidato);
    }

    /// <summary>Retorna candidatos de um processo</summary>
    [HttpGet("processo/{processoId}")]
    public async Task<ActionResult<IEnumerable<CandidatoDto>>> GetByProcessoId(long processoId)
    {
        var candidatos = await _service.GetByProcessoIdAsync(processoId);
        return Ok(candidatos);
    }

    /// <summary>Retorna a pontuação de um candidato</summary>
    [HttpGet("{id}/pontuacao")]
    public async Task<ActionResult<float>> GetPontuacao(long id)
    {
        var pontuacao = await _service.GetPontuacaoAsync(id);
        return Ok(pontuacao);
    }

    /// <summary>Cria um novo candidato</summary>
    [HttpPost]
    public async Task<ActionResult<CandidatoDto>> Create([FromBody] CreateCandidatoDto dto)
    {
        var candidato = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = candidato.Id }, candidato);
    }

    /// <summary>Atualiza um candidato</summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<CandidatoDto>> Update(long id, [FromBody] UpdateCandidatoDto dto)
    {
        try
        {
            var candidato = await _service.UpdateAsync(id, dto);
            return Ok(candidato);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>Remove um candidato</summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
