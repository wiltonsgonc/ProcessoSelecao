using Microsoft.AspNetCore.Mvc;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Application.Services;

namespace ProcessoSelecao.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProcessosSelecaoController : ControllerBase
{
    private readonly IProcessoSelecaoService _service;

    public ProcessosSelecaoController(IProcessoSelecaoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProcessoSelecaoDto>>> GetAll()
    {
        var processos = await _service.GetAllAsync();
        return Ok(processos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProcessoSelecaoDto>> GetById(long id)
    {
        var processo = await _service.GetByIdAsync(id);
        if (processo == null) return NotFound();
        return Ok(processo);
    }

    [HttpPost]
    public async Task<ActionResult<ProcessoSelecaoDto>> Create([FromBody] CreateProcessoSelecaoDto dto)
    {
        var processo = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = processo.Id }, processo);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProcessoSelecaoDto>> Update(long id, [FromBody] UpdateProcessoSelecaoDto dto)
    {
        try
        {
            var processo = await _service.UpdateAsync(id, dto);
            return Ok(processo);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/iniciar")]
    public async Task<ActionResult<ProcessoSelecaoDto>> Iniciar(long id)
    {
        try
        {
            var processo = await _service.IniciarAsync(id);
            return Ok(processo);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/finalizar")]
    public async Task<ActionResult<ProcessoSelecaoDto>> Finalizar(long id)
    {
        try
        {
            var processo = await _service.FinalizarAsync(id);
            return Ok(processo);
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
