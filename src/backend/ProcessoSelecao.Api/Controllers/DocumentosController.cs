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
            var documento = await _service.GetByIdAsync(id);
            if (documento == null) return NotFound("Documento não encontrado");
            
            var filePath = await _service.GetFilePathAsync(id);
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/pdf", documento.NomeArquivo, enableRangeProcessing: true);
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
            var documento = await _service.GetByIdAsync(id);
            if (documento == null) return NotFound("Documento não encontrado");
            
            var filePath = await _service.GetFilePathAsync(id);
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/pdf", documento.NomeArquivo, enableRangeProcessing: true);
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

    /// <summary>Cria novo documento com URL (Currículo Lattes)</summary>
    [HttpPost("with-url")]
    public async Task<ActionResult<DocumentoDto>> CreateWithUrl([FromBody] CreateDocumentoWithUrlDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.LinkUrl))
            return BadRequest("LinkUrl é obrigatório");

        var documento = await _service.CreateWithUrlAsync(dto);
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

    /// <summary>Download múltiplos documentos em ZIP</summary>
    [HttpPost("download-multiple")]
    public async Task<ActionResult> DownloadMultiple([FromBody] List<long> ids)
    {
        if (ids == null || ids.Count == 0)
            return BadRequest("Nenhum documento selecionado");

        var zipBytes = await CreateZipAsync(ids);
        return File(zipBytes, "application/octet-stream", "documentos_selecionados.zip");
    }

    private async Task<byte[]> CreateZipAsync(List<long> ids)
    {
        using var memoryStream = new MemoryStream();
        using (var archive = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, false))
        {
            foreach (var id in ids)
            {
                try
                {
                    var documento = await _service.GetByIdAsync(id);
                    if (documento == null) continue;
                    
                    var filePath = await _service.GetFilePathAsync(id);
                    var entry = archive.CreateEntry(documento.NomeArquivo, System.IO.Compression.CompressionLevel.Optimal);
                    using var entryStream = entry.Open();
                    using var fileStream = System.IO.File.OpenRead(filePath);
                    await fileStream.CopyToAsync(entryStream);
                }
                catch
                {
                    // Ignora documentos não encontrados
                }
            }
        }
        return memoryStream.ToArray();
    }
}
