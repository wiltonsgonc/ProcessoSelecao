using Microsoft.AspNetCore.Mvc;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Application.Services;

namespace ProcessoSelecao.Api.Controllers;

/// <summary>
/// Controller para manipulação de Documentos
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DocumentosController : ControllerBase
{
    private readonly IDocumentoService _service;
    private readonly IConfiguration _configuration;

    public DocumentosController(IDocumentoService service, IConfiguration configuration)
    {
        _service = service;
        _configuration = configuration;
    }

    /// <summary>Retorna todos os documentos</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DocumentoDto>>> GetAll()
    {
        var documentos = await _service.GetAllAsync();
        return Ok(documentos);
    }

    /// <summary>Retorna um documento pelo ID</summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentoDto>> GetById(long id)
    {
        var documento = await _service.GetByIdAsync(id);
        if (documento == null) return NotFound();
        return Ok(documento);
    }

    /// <summary>Retorna documentos de um candidato</summary>
    [HttpGet("candidato/{candidatoId}")]
    public async Task<ActionResult<IEnumerable<DocumentoDto>>> GetByCandidatoId(long candidatoId)
    {
        var documentos = await _service.GetByCandidatoIdAsync(candidatoId);
        return Ok(documentos);
    }

    /// <summary>Download de um documento</summary>
    [HttpGet("{id}/download")]
    public async Task<ActionResult> Download(long id)
    {
        try
        {
            var filePath = await _service.GetFilePathAsync(id);
            var fileName = Path.GetFileName(filePath);
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/pdf", fileName, enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>Visualizar documento PDF no navegador</summary>
    [HttpGet("{id}/view")]
    [Produces("application/pdf")]
    public async Task<IActionResult> View(long id)
    {
        try
        {
            var filePath = await _service.GetFilePathAsync(id);
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var fileName = Path.GetFileName(filePath);
            return File(fileBytes, "application/pdf", fileName, enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>Upload de um novo documento</summary>
    [HttpPost]
    public async Task<ActionResult<DocumentoDto>> Create([FromForm] CreateDocumentoDto dto, IFormFile arquivo)
    {
        if (arquivo == null || arquivo.Length == 0)
            return BadRequest("Arquivo é obrigatório");

        var caminhoBase = _configuration["Storage:CaminhoBase"] ?? "/app/documentos";
        
        using var stream = arquivo.OpenReadStream();
        var documento = await _service.CreateAsync(dto, stream, caminhoBase);
        return CreatedAtAction(nameof(GetById), new { id = documento.Id }, documento);
    }

    /// <summary>Valida um documento</summary>
    [HttpPut("{id}/validar")]
    public async Task<ActionResult<DocumentoDto>> Validate(long id, [FromBody] ValidateDocumentoDto dto)
    {
        try
        {
            var documento = await _service.ValidateAsync(id, dto);
            return Ok(documento);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>Remove um documento</summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
