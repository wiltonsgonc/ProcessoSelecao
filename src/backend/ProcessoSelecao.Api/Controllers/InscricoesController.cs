using Microsoft.AspNetCore.Mvc;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Application.Services;

namespace ProcessoSelecao.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InscricoesController : ControllerBase
{
    private readonly InscricaoService _inscricaoService;

    public InscricoesController(InscricaoService inscricaoService)
    {
        _inscricaoService = inscricaoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InscricaoDto>>> GetAll()
    {
        var inscricoes = await _inscricaoService.GetAllAsync();
        return Ok(inscricoes);
    }

    [HttpGet("edital/{editalId}")]
    public async Task<ActionResult<IEnumerable<InscricaoDto>>> GetByEdital(int editalId)
    {
        var inscricoes = await _inscricaoService.GetByEditalIdAsync(editalId);
        return Ok(inscricoes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InscricaoDto>> GetById(int id)
    {
        var inscricao = await _inscricaoService.GetByIdAsync(id);
        if (inscricao == null) return NotFound();
        return Ok(inscricao);
    }

    [HttpPost]
    public async Task<ActionResult<InscricaoDto>> Create([FromBody] InscricaoCreateDto createDto)
    {
        try
        {
            var inscricao = await _inscricaoService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = inscricao.Id }, inscricao);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<InscricaoDto>> Update(int id, [FromBody] InscricaoCreateDto updateDto)
    {
        try
        {
            var inscricao = await _inscricaoService.UpdateAsync(id, updateDto);
            if (inscricao == null) return NotFound();
            return Ok(inscricao);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/confirmar")]
    public async Task<ActionResult> Confirmar(int id)
    {
        try
        {
            var result = await _inscricaoService.ConfirmarAsync(id);
            if (!result) return NotFound();
            return Ok(new { message = "Inscrição confirmada com sucesso" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/cancelar")]
    public async Task<ActionResult> Cancelar(int id)
    {
        var result = await _inscricaoService.CancelarAsync(id);
        if (!result) return NotFound();
        return Ok(new { message = "Inscrição cancelada com sucesso" });
    }

    [HttpPost("{id}/validar-documentos")]
    public async Task<ActionResult> ValidarDocumentos(int id)
    {
        try
        {
            var result = await _inscricaoService.ValidarDocumentosAsync(id);
            return Ok(new { valido = result });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
