using Microsoft.AspNetCore.Mvc;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Application.Services;

namespace ProcessoSelecao.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AvaliadoresController : ControllerBase
{
    private readonly IAvaliadorService _service;

    public AvaliadoresController(IAvaliadorService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AvaliadorDto>>> GetAll()
    {
        var avaliadores = await _service.GetAllAsync();
        return Ok(avaliadores);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AvaliadorDto>> GetById(long id)
    {
        var avaliador = await _service.GetByIdAsync(id);
        if (avaliador == null) return NotFound();
        return Ok(avaliador);
    }

    [HttpGet("processo/{processoId}")]
    public async Task<ActionResult<IEnumerable<AvaliadorDto>>> GetByProcessoId(long processoId)
    {
        var avaliadores = await _service.GetByProcessoIdAsync(processoId);
        return Ok(avaliadores);
    }

    [HttpPost]
    public async Task<ActionResult<AvaliadorDto>> Create([FromBody] CreateAvaliadorDto dto)
    {
        var avaliador = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = avaliador.Id }, avaliador);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AvaliadorDto>> Update(long id, [FromBody] UpdateAvaliadorDto dto)
    {
        try
        {
            var avaliador = await _service.UpdateAsync(id, dto);
            return Ok(avaliador);
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
