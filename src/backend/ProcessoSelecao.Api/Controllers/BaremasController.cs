using Microsoft.AspNetCore.Mvc;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Application.Services;

namespace ProcessoSelecao.Api.Controllers;

/// <summary>
/// Controller para manipulação de Baremas/Avaliações
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BaremasController : ControllerBase
{
    private readonly IBaremaService _service;

    public BaremasController(IBaremaService service)
    {
        _service = service;
    }

    /// <summary>Retorna todos os baremas</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BaremaDto>>> GetAll()
    {
        var baremas = await _service.GetAllAsync();
        return Ok(baremas);
    }

    /// <summary>Retorna um barema pelo ID</summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BaremaDto>> GetById(long id)
    {
        var barema = await _service.GetByIdAsync(id);
        if (barema == null) return NotFound();
        return Ok(barema);
    }

    /// <summary>Retorna baremas de um candidato</summary>
    [HttpGet("candidato/{candidatoId}")]
    public async Task<ActionResult<IEnumerable<BaremaDto>>> GetByCandidatoId(long candidatoId)
    {
        var baremas = await _service.GetByCandidatoIdAsync(candidatoId);
        return Ok(baremas);
    }

    /// <summary>Retorna baremas de um avaliador</summary>
    [HttpGet("avaliador/{avaliadorId}")]
    public async Task<ActionResult<IEnumerable<BaremaDto>>> GetByAvaliadorId(long avaliadorId)
    {
        var baremas = await _service.GetByAvaliadorIdAsync(avaliadorId);
        return Ok(baremas);
    }

    /// <summary>Cria um novo barema</summary>
    [HttpPost]
    public async Task<ActionResult<BaremaDto>> Create([FromBody] CreateBaremaDto dto)
    {
        var barema = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = barema.Id }, barema);
    }

    /// <summary>Atualiza critérios de um barema</summary>
    [HttpPut("{id}/criterios")]
    public async Task<ActionResult<BaremaDto>> UpdateCriterios(long id, [FromBody] UpdateBaremaDto dto)
    {
        try
        {
            var barema = await _service.UpdateCriteriosAsync(id, dto);
            return Ok(barema);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>Finaliza um barema</summary>
    [HttpPost("{id}/finalizar")]
    public async Task<ActionResult<BaremaDto>> Finalizar(long id, [FromBody] FinalizarBaremaDto dto)
    {
        try
        {
            var barema = await _service.FinalizarAsync(id, dto);
            return Ok(barema);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>Remove um barema</summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
