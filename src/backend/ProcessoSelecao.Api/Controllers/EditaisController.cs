using Microsoft.AspNetCore.Mvc;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Application.Services;

namespace ProcessoSelecao.Api.Controllers;

/// <summary>
/// Controller para manipulação de Editais
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EditaisController : ControllerBase
{
    private readonly EditalService _editalService;

    public EditaisController(EditalService editalService)
    {
        _editalService = editalService;
    }

    /// <summary>
    /// Retorna todos os editais
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EditalDto>>> GetAll()
    {
        var editais = await _editalService.GetAllAsync();
        return Ok(editais);
    }

    /// <summary>
    /// Retorna apenas editais publicados
    /// </summary>
    [HttpGet("publicados")]
    public async Task<ActionResult<IEnumerable<EditalDto>>> GetPublished()
    {
        var editais = await _editalService.GetPublishedAsync();
        return Ok(editais);
    }

    /// <summary>
    /// Retorna um edital pelo ID
    /// </summary>
    /// <param name="id">ID do edital</param>
    [HttpGet("{id}")]
    public async Task<ActionResult<EditalDto>> GetById(int id)
    {
        var edital = await _editalService.GetByIdAsync(id);
        if (edital == null) return NotFound();
        return Ok(edital);
    }

    /// <summary>
    /// Cria um novo edital
    /// </summary>
    /// <param name="createDto">Dados do edital</param>
    [HttpPost]
    public async Task<ActionResult<EditalDto>> Create([FromBody] EditalCreateDto createDto)
    {
        var edital = await _editalService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = edital.Id }, edital);
    }

    /// <summary>
    /// Atualiza um edital existente
    /// </summary>
    /// <param name="id">ID do edital</param>
    /// <param name="updateDto">Dados atualizados</param>
    [HttpPut("{id}")]
    public async Task<ActionResult<EditalDto>> Update(int id, [FromBody] EditalUpdateDto updateDto)
    {
        if (id != updateDto.Id) return BadRequest();
        var edital = await _editalService.UpdateAsync(updateDto);
        if (edital == null) return NotFound();
        return Ok(edital);
    }

    /// <summary>
    /// Remove um edital
    /// </summary>
    /// <param name="id">ID do edital</param>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _editalService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Publica um edital (altera status para publicado)
    /// </summary>
    /// <param name="id">ID do edital</param>
    [HttpPost("{id}/publicar")]
    public async Task<ActionResult> Publicar(int id)
    {
        var result = await _editalService.PublicarAsync(id);
        if (!result) return NotFound();
        return Ok(new { message = "Edital publicado com sucesso" });
    }

    /// <summary>
    /// Encerra um edital
    /// </summary>
    /// <param name="id">ID do edital</param>
    [HttpPost("{id}/encerrar")]
    public async Task<ActionResult> Encerrar(int id)
    {
        var result = await _editalService.EncerrarAsync(id);
        if (!result) return NotFound();
        return Ok(new { message = "Edital encerrado com sucesso" });
    }
}
