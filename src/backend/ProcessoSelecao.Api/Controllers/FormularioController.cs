using Microsoft.AspNetCore.Mvc;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Application.Services;
using System.Text.Json;

namespace ProcessoSelecao.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FormularioController : ControllerBase
{
    private readonly IInscricaoService _inscricaoService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FormularioController> _logger;

    public FormularioController(IInscricaoService inscricaoService, IConfiguration configuration, ILogger<FormularioController> logger)
    {
        _inscricaoService = inscricaoService;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("pagina1")]
    public IActionResult PostPagina1([FromBody] Pagina1Dto dados)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(new { message = "Dados da Página 1 recebidos com sucesso!" });
    }

    [HttpPost("pagina2")]
    public IActionResult PostPagina2([FromBody] Pagina2Dto dados)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(new { message = "Dados da Página 2 recebidos com sucesso!" });
    }

    [HttpPost("pagina3")]
    public IActionResult PostPagina3()
    {
        return Ok(new { message = "Dados da Página 3 devem ser enviados junto com a inscrição completa" });
    }

    [HttpPost("pagina4")]
    public IActionResult PostPagina4([FromBody] Pagina4Dto dados)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(new { message = "Dados da Página 4 recebidos com sucesso!" });
    }

    [HttpPost("completa")]
    public async Task<IActionResult> PostInscricaoCompleta()
    {
        try
        {
            var form = await Request.ReadFormAsync();
            _logger.LogInformation("Recebida requisição de inscrição completa. Processo: {ProcessoId}", form["processoSelecaoId"].FirstOrDefault());
            
            var processoSelecaoIdStr = form["processoSelecaoId"].FirstOrDefault();
            if (!long.TryParse(processoSelecaoIdStr, out var processoSelecaoId))
                return BadRequest(new { message = "ID do processo de seleção inválido" });

            var dadosJson = form["dados"].FirstOrDefault();
            if (string.IsNullOrEmpty(dadosJson))
                return BadRequest(new { message = "Dados da inscrição não fornecidos" });

            var dados = JsonSerializer.Deserialize<CreateInscricaoCompletaDto>(dadosJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (dados == null)
                return BadRequest(new { message = "Formato dos dados inválido" });

            dados.ProcessoSelecaoId = processoSelecaoId;

            var documentos = new List<DocumentoUploadDto>();
            var documentosLink = new List<DocumentoLinkDto>();
            
            foreach (var file in form.Files)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                var tipo = MapearNomeCampoParaTipoDocumento(file.Name);
                documentos.Add(new DocumentoUploadDto
                {
                    Tipo = tipo,
                    NomeArquivo = file.FileName,
                    Arquivo = ms.ToArray()
                });
            }

            var curriculoLattesCandidato = form["curriculoLattesCandidato"].FirstOrDefault();
            var curriculoLattesOrientador = form["curriculoLattesOrientador"].FirstOrDefault();
            
            if (!string.IsNullOrWhiteSpace(curriculoLattesCandidato))
            {
                documentosLink.Add(new DocumentoLinkDto
                {
                    Tipo = Domain.Enums.TipoDocumento.CurriculumLatte,
                    LinkUrl = curriculoLattesCandidato,
                    Descricao = "Currículo Lattes Candidato"
                });
            }
            
            if (!string.IsNullOrWhiteSpace(curriculoLattesOrientador))
            {
                documentosLink.Add(new DocumentoLinkDto
                {
                    Tipo = Domain.Enums.TipoDocumento.CartaRecomendacao,
                    LinkUrl = curriculoLattesOrientador,
                    Descricao = "Currículo Lattes Orientador"
                });
            }

            dados.Documentos = documentos;
            dados.DocumentosLink = documentosLink;

            var caminhoBase = _configuration["Storage:CaminhoBase"] ?? "/app/documentos";
            var resultado = await _inscricaoService.CriarInscricaoCompletaAsync(dados, caminhoBase);

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar inscrição completa");
            return StatusCode(500, new { message = "Erro ao processar inscrição", erro = ex.Message });
        }
    }

    private static Domain.Enums.TipoDocumento MapearNomeCampoParaTipoDocumento(string nomeCampo)
    {
        var nome = nomeCampo.ToLower().Replace("-", "").Replace("_", "");
        return nome switch
        {
            "rgcpfcandidato" => Domain.Enums.TipoDocumento.HistoricoEscolar,
            "anexoi" => Domain.Enums.TipoDocumento.ComprovanteMatricula,
            "curriculolattescandidato" => Domain.Enums.TipoDocumento.CurriculumLatte,
            "curriculolattesorientador" => Domain.Enums.TipoDocumento.CartaRecomendacao,
            "anexoii" => Domain.Enums.TipoDocumento.CartaIntencao,
            "comprovantematricula" => Domain.Enums.TipoDocumento.ComprovanteMatricula,
            "historicoescolar" => Domain.Enums.TipoDocumento.HistoricoEscolar,
            _ => Domain.Enums.TipoDocumento.HistoricoEscolar
        };
    }
}
