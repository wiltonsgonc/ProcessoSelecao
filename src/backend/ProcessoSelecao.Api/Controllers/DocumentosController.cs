using Microsoft.AspNetCore.Mvc;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Application.Services;

namespace ProcessoSelecao.Api.Controllers;

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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DocumentoDto>>> GetAll()
    {
        var documentos = await _service.GetAllAsync();
        return Ok(documentos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentoDto>> GetById(long id)
    {
        var documento = await _service.GetByIdAsync(id);
        if (documento == null) return NotFound();
        return Ok(documento);
    }

    [HttpGet("candidato/{candidatoId}")]
    public async Task<ActionResult<IEnumerable<DocumentoDto>>> GetByCandidatoId(long candidatoId)
    {
        var documentos = await _service.GetByCandidatoIdAsync(candidatoId);
        return Ok(documentos);
    }

    [HttpGet("{id}/download")]
    public async Task<ActionResult> Download(long id)
    {
        try
        {
            var filePath = await _service.GetFilePathAsync(id);
            var fileName = Path.GetFileName(filePath);
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/octet-stream", fileName);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

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

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
