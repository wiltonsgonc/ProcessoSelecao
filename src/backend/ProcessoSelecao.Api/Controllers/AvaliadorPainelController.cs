using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Application.Services;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AvaliadorPainelController : ControllerBase
{
    private readonly IBaremaService _baremaService;
    private readonly ICandidatoService _candidatoService;
    private readonly IDocumentoService _documentoService;
    private readonly IAvaliadorService _avaliadorService;

    public AvaliadorPainelController(
        IBaremaService baremaService,
        ICandidatoService candidatoService,
        IDocumentoService documentoService,
        IAvaliadorService avaliadorService)
    {
        _baremaService = baremaService;
        _candidatoService = candidatoService;
        _documentoService = documentoService;
        _avaliadorService = avaliadorService;
    }

    private long ObterAvaliadorId()
    {
        var claim = User.FindFirst("avaliadorId");
        if (claim == null) throw new UnauthorizedAccessException();
        return long.Parse(claim.Value);
    }

    [HttpGet("baremas")]
    public async Task<ActionResult<IEnumerable<BaremaDto>>> GetMeusBaremas()
    {
        var avaliadorId = ObterAvaliadorId();
        var baremas = await _baremaService.GetByAvaliadorIdAsync(avaliadorId);
        return Ok(baremas);
    }

    [HttpGet("barema-dados/{baremaId}")]
    public async Task<ActionResult<BaremaDadosDto>> GetDadosBarema(long baremaId)
    {
        var dados = await _baremaService.GetDadosBaremaAsync(baremaId);
        if (dados == null) return NotFound();
        return Ok(dados);
    }

    [HttpGet("candidato/{candidatoId}")]
    public async Task<ActionResult<CandidatoDto>> GetCandidato(long candidatoId)
    {
        var candidato = await _candidatoService.GetByIdAsync(candidatoId);
        if (candidato == null) return NotFound();
        return Ok(candidato);
    }

    [HttpGet("documentos/{candidatoId}")]
    public async Task<ActionResult<IEnumerable<DocumentoDto>>> GetDocumentosCandidato(long candidatoId)
    {
        var documentos = await _documentoService.GetByCandidatoIdAsync(candidatoId);
        return Ok(documentos);
    }

    [HttpGet("documentos/{id}/download")]
    public async Task<IActionResult> DownloadDocumento(long id)
    {
        var documento = await _documentoService.GetByIdAsync(id);
        if (documento == null) return NotFound();

        var filePath = await _documentoService.GetFilePathAsync(id);
        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        return File(fileBytes, "application/pdf", documento.NomeArquivo, enableRangeProcessing: true);
    }

    [HttpPost("baremas/{id}/finalizar")]
    public async Task<ActionResult<BaremaDto>> FinalizarAvaliacao(long id, [FromBody] FinalizarBaremaDto dto)
    {
        var avaliadorId = ObterAvaliadorId();
        var resultado = await _baremaService.FinalizarAsync(id, dto);
        return Ok(resultado);
    }

    [HttpPost("baremas/{id}/finalizar-com-template")]
    public async Task<ActionResult<BaremaDto>> FinalizarComTemplate(long id, [FromBody] FinalizarBaremaTemplateDto dto)
    {
        var avaliadorId = ObterAvaliadorId();
        var resultado = await _baremaService.FinalizarComTemplateAsync(id, dto);
        return Ok(resultado);
    }

    [HttpPost("baremas/{id}/finalizar-eliminacao")]
    public async Task<ActionResult<BaremaDto>> FinalizarPorEliminacao(long id)
    {
        var resultado = await _baremaService.FinalizarPorEliminacaoAsync(id);
        return Ok(resultado);
    }
}
