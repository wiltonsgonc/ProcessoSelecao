using Microsoft.AspNetCore.Mvc;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Application.Services;

namespace ProcessoSelecao.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EditaisController : ControllerBase
{
    private readonly EditalService _editalService;

    public EditaisController(EditalService editalService)
    {
        _editalService = editalService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EditalDto>>> GetAll()
    {
        var editais = await _editalService.GetAllAsync();
        return Ok(editais);
    }

    [HttpGet("publicados")]
    public async Task<ActionResult<IEnumerable<EditalDto>>> GetPublished()
    {
        var editais = await _editalService.GetPublishedAsync();
        return Ok(editais);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EditalDto>> GetById(int id)
    {
        var edital = await _editalService.GetByIdAsync(id);
        if (edital == null) return NotFound();
        return Ok(edital);
    }

    [HttpPost]
    public async Task<ActionResult<EditalDto>> Create([FromBody] EditalCreateDto createDto)
    {
        var edital = await _editalService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = edital.Id }, edital);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<EditalDto>> Update(int id, [FromBody] EditalUpdateDto updateDto)
    {
        if (id != updateDto.Id) return BadRequest();
        var edital = await _editalService.UpdateAsync(updateDto);
        if (edital == null) return NotFound();
        return Ok(edital);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _editalService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPost("{id}/publicar")]
    public async Task<ActionResult> Publicar(int id)
    {
        var result = await _editalService.PublicarAsync(id);
        if (!result) return NotFound();
        return Ok(new { message = "Edital publicado com sucesso" });
    }

    [HttpPost("{id}/encerrar")]
    public async Task<ActionResult> Encerrar(int id)
    {
        var result = await _editalService.EncerrarAsync(id);
        if (!result) return NotFound();
        return Ok(new { message = "Edital encerrado com sucesso" });
    }
}
