using Microsoft.AspNetCore.Mvc;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Application.Services;

namespace ProcessoSelecao.Api.Controllers;

/// <summary>
/// Controller para manipulação de Processos de Seleção
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProcessosSelecaoController : ControllerBase
{
    private readonly IProcessoSelecaoService _service;

    public ProcessosSelecaoController(IProcessoSelecaoService service)
    {
        _service = service;
    }

    /// <summary>Retorna todos os processos</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProcessoSelecaoDto>>> GetAll()
    {
        var processos = await _service.GetAllAsync();
        return Ok(processos);
    }

    /// <summary>Retorna um processo pelo ID</summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProcessoSelecaoDto>> GetById(long id)
    {
        var processo = await _service.GetByIdAsync(id);
        if (processo == null) return NotFound();
        return Ok(processo);
    }

    /// <summary>Cria um novo processo</summary>
    [HttpPost]
    public async Task<ActionResult<ProcessoSelecaoDto>> Create([FromBody] CreateProcessoSelecaoDto dto)
    {
        var processo = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = processo.Id }, processo);
    }

    /// <summary>Atualiza um processo</summary>
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

    /// <summary>Inicia um processo</summary>
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

    /// <summary>Finaliza um processo</summary>
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

    /// <summary>Remove um processo</summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
