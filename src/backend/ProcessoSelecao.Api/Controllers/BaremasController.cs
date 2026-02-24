using Microsoft.AspNetCore.Mvc;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Application.Services;

namespace ProcessoSelecao.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaremasController : ControllerBase
{
    private readonly IBaremaService _service;

    public BaremasController(IBaremaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BaremaDto>>> GetAll()
    {
        var baremas = await _service.GetAllAsync();
        return Ok(baremas);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BaremaDto>> GetById(long id)
    {
        var barema = await _service.GetByIdAsync(id);
        if (barema == null) return NotFound();
        return Ok(barema);
    }

    [HttpGet("candidato/{candidatoId}")]
    public async Task<ActionResult<IEnumerable<BaremaDto>>> GetByCandidatoId(long candidatoId)
    {
        var baremas = await _service.GetByCandidatoIdAsync(candidatoId);
        return Ok(baremas);
    }

    [HttpGet("avaliador/{avaliadorId}")]
    public async Task<ActionResult<IEnumerable<BaremaDto>>> GetByAvaliadorId(long avaliadorId)
    {
        var baremas = await _service.GetByAvaliadorIdAsync(avaliadorId);
        return Ok(baremas);
    }

    [HttpPost]
    public async Task<ActionResult<BaremaDto>> Create([FromBody] CreateBaremaDto dto)
    {
        var barema = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = barema.Id }, barema);
    }

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

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
