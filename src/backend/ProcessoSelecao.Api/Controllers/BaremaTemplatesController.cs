using Microsoft.AspNetCore.Mvc;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Application.Services;

namespace ProcessoSelecao.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaremaTemplatesController : ControllerBase
{
    private readonly IBaremaTemplateService _templateService;

    public BaremaTemplatesController(IBaremaTemplateService templateService)
    {
        _templateService = templateService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BaremaTemplateDto>>> GetAll()
    {
        var templates = await _templateService.GetAllAsync();
        return Ok(templates);
    }

    [HttpGet("ativas")]
    public async Task<ActionResult<IEnumerable<BaremaTemplateDto>>> GetActive()
    {
        var templates = await _templateService.GetActiveAsync();
        return Ok(templates);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BaremaTemplateDto>> GetById(long id)
    {
        var template = await _templateService.GetByIdAsync(id);
        if (template == null) return NotFound();
        return Ok(template);
    }

    [HttpPost]
    public async Task<ActionResult<BaremaTemplateDto>> Create([FromBody] CreateBaremaTemplateDto dto)
    {
        try
        {
            var created = await _templateService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BaremaTemplateDto>> Update(long id, [FromBody] UpdateBaremaTemplateDto dto)
    {
        try
        {
            var updated = await _templateService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/clone")]
    public async Task<ActionResult<BaremaTemplateDto>> Clone(long id, [FromBody] CloneTemplateRequest request)
    {
        try
        {
            var clone = await _templateService.CloneAsync(id, request.Nome);
            if (clone == null) return NotFound();
            return Ok(clone);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _templateService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPut("{id}/toggle-ativo")]
    public async Task<ActionResult<BaremaTemplateDto>> ToggleAtivo(long id)
    {
        var result = await _templateService.ToggleAtivoAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }
}

public class CloneTemplateRequest
{
    public string Nome { get; set; } = string.Empty;
}
