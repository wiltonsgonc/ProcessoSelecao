using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;
using ProcessoSelecao.Domain.Interfaces;
using ProcessoSelecao.Infrastructure.Data;

namespace ProcessoSelecao.Api.Controllers;

/// <summary>
/// Controller para manipulação de Documentos de Inscrição
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DocumentosInscricaoController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public DocumentosInscricaoController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    /// <summary>
    /// Realiza upload de um documento para uma inscrição
    /// </summary>
    /// <param name="file">Arquivo a ser enviado</param>
    /// <param name="inscricaoId">ID da inscrição</param>
    /// <param name="tipoDocumento">Tipo do documento (enum TipoDocumentoInscricao)</param>
    [HttpPost("upload")]
    public async Task<ActionResult<DocumentoInscricaoDto>> Upload(IFormFile file, int inscricaoId, int tipoDocumento)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "Arquivo não fornecido" });

        var allowedTypes = new[] { "application/pdf", "image/jpeg", "image/png", "image/jpg" };
        if (!allowedTypes.Contains(file.ContentType.ToLowerInvariant()))
            return BadRequest(new { message = "Apenas arquivos PDF, JPG ou PNG são permitidos" });

        if (file.Length > 10 * 1024 * 1024)
            return BadRequest(new { message = "O arquivo não pode exceder 10MB" });

        var inscricao = await _context.Inscricoes.FindAsync(inscricaoId);
        if (inscricao == null)
            return NotFound(new { message = "Inscrição não encontrada" });

        var uploadsPath = Path.Combine(_env.ContentRootPath, "uploads", "inscricoes", inscricaoId.ToString());
        Directory.CreateDirectory(uploadsPath);

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(uploadsPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var documento = new DocumentoInscricao
        {
            InscricaoId = inscricaoId,
            Tipo = (TipoDocumentoInscricao)tipoDocumento,
            NomeArquivoOriginal = file.FileName,
            NomeArquivoSalvo = fileName,
            CaminhoLocal = filePath,
            TamanhoBytes = file.Length,
            ContentType = file.ContentType,
            DataUpload = DateTime.UtcNow
        };

        _context.DocumentosInscricao.Add(documento);
        await _context.SaveChangesAsync();

        var dto = new DocumentoInscricaoDto
        {
            Id = documento.Id,
            InscricaoId = documento.InscricaoId,
            Tipo = documento.Tipo,
            NomeArquivoOriginal = documento.NomeArquivoOriginal,
            TamanhoBytes = documento.TamanhoBytes,
            DataUpload = documento.DataUpload
        };

        return Ok(dto);
    }

    /// <summary>
    /// Retorna todos os documentos de uma inscrição
    /// </summary>
    /// <param name="inscricaoId">ID da inscrição</param>
    [HttpGet("inscricao/{inscricaoId}")]
    public async Task<ActionResult<IEnumerable<DocumentoInscricaoDto>>> GetByInscricao(int inscricaoId)
    {
        var documentos = await _context.DocumentosInscricao
            .Where(d => d.InscricaoId == inscricaoId)
            .Select(d => new DocumentoInscricaoDto
            {
                Id = (int)d.Id,
                InscricaoId = d.InscricaoId,
                Tipo = d.Tipo,
                NomeArquivoOriginal = d.NomeArquivoOriginal,
                TamanhoBytes = d.TamanhoBytes,
                DataUpload = d.DataUpload
            })
            .ToListAsync();

        return Ok(documentos);
    }

    /// <summary>
    /// Retorna um documento pelo ID
    /// </summary>
    /// <param name="id">ID do documento</param>
    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentoInscricaoDto>> GetById(int id)
    {
        var documento = await _context.DocumentosInscricao.FindAsync(id);
        if (documento == null)
            return NotFound();

        var dto = new DocumentoInscricaoDto
        {
            Id = documento.Id,
            InscricaoId = documento.InscricaoId,
            Tipo = documento.Tipo,
            NomeArquivoOriginal = documento.NomeArquivoOriginal,
            TamanhoBytes = documento.TamanhoBytes,
            DataUpload = documento.DataUpload
        };

        return Ok(dto);
    }

    /// <summary>
    /// Remove um documento
    /// </summary>
    /// <param name="id">ID do documento</param>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var documento = await _context.DocumentosInscricao.FindAsync(id);
        if (documento == null)
            return NotFound();

        if (System.IO.File.Exists(documento.CaminhoLocal))
        {
            System.IO.File.Delete(documento.CaminhoLocal);
        }

        _context.DocumentosInscricao.Remove(documento);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
