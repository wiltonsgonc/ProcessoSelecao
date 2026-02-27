using Microsoft.AspNetCore.Mvc;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Application.Services;

namespace ProcessoSelecao.Api.Controllers;

/// <summary>
/// Controller para manipulação de Inscrições
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InscricoesController : ControllerBase
{
    private readonly InscricaoService _inscricaoService;

    public InscricoesController(InscricaoService inscricaoService)
    {
        _inscricaoService = inscricaoService;
    }

    /// <summary>
    /// Retorna todas as inscrições
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InscricaoDto>>> GetAll()
    {
        var inscricoes = await _inscricaoService.GetAllAsync();
        return Ok(inscricoes);
    }

    /// <summary>
    /// Retorna todas as inscrições de um edital específico
    /// </summary>
    /// <param name="editalId">ID do edital</param>
    [HttpGet("edital/{editalId}")]
    public async Task<ActionResult<IEnumerable<InscricaoDto>>> GetByEdital(int editalId)
    {
        var inscricoes = await _inscricaoService.GetByEditalIdAsync(editalId);
        return Ok(inscricoes);
    }

    /// <summary>
    /// Retorna uma inscrição pelo ID
    /// </summary>
    /// <param name="id">ID da inscrição</param>
    [HttpGet("{id}")]
    public async Task<ActionResult<InscricaoDto>> GetById(int id)
    {
        var inscricao = await _inscricaoService.GetByIdAsync(id);
        if (inscricao == null) return NotFound();
        return Ok(inscricao);
    }

    /// <summary>
    /// Cria uma nova inscrição
    /// </summary>
    /// <param name="createDto">Dados da inscrição</param>
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

    /// <summary>
    /// Atualiza uma inscrição existente
    /// </summary>
    /// <param name="id">ID da inscrição</param>
    /// <param name="updateDto">Dados atualizados</param>
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

    /// <summary>
    /// Confirma uma inscrição
    /// </summary>
    /// <param name="id">ID da inscrição</param>
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

    /// <summary>
    /// Cancela uma inscrição
    /// </summary>
    /// <param name="id">ID da inscrição</param>
    [HttpPost("{id}/cancelar")]
    public async Task<ActionResult> Cancelar(int id)
    {
        var result = await _inscricaoService.CancelarAsync(id);
        if (!result) return NotFound();
        return Ok(new { message = "Inscrição cancelada com sucesso" });
    }

    /// <summary>
    /// Valida os documentos de uma inscrição
    /// </summary>
    /// <param name="id">ID da inscrição</param>
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
