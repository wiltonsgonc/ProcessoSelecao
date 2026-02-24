using Microsoft.AspNetCore.Mvc;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Application.Services;

namespace ProcessoSelecao.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CandidatosController : ControllerBase
{
    private readonly ICandidatoService _service;

    public CandidatosController(ICandidatoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CandidatoDto>>> GetAll()
    {
        var candidatos = await _service.GetAllAsync();
        return Ok(candidatos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CandidatoDto>> GetById(long id)
    {
        var candidato = await _service.GetByIdAsync(id);
        if (candidato == null) return NotFound();
        return Ok(candidato);
    }

    [HttpGet("processo/{processoId}")]
    public async Task<ActionResult<IEnumerable<CandidatoDto>>> GetByProcessoId(long processoId)
    {
        var candidatos = await _service.GetByProcessoIdAsync(processoId);
        return Ok(candidatos);
    }

    [HttpGet("{id}/pontuacao")]
    public async Task<ActionResult<float>> GetPontuacao(long id)
    {
        var pontuacao = await _service.GetPontuacaoAsync(id);
        return Ok(pontuacao);
    }

    [HttpPost]
    public async Task<ActionResult<CandidatoDto>> Create([FromBody] CreateCandidatoDto dto)
    {
        var candidato = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = candidato.Id }, candidato);
    }

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

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
