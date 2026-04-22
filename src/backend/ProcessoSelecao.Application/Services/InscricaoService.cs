using AutoMapper;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;
using ProcessoSelecao.Domain.Interfaces;
using System.Security.Cryptography;

namespace ProcessoSelecao.Application.Services;

public interface IInscricaoService
{
    Task<InscricaoResultDto> CriarInscricaoCompletaAsync(CreateInscricaoCompletaDto dto, string caminhoBase);
}

public class InscricaoService : IInscricaoService
{
    private readonly ICandidatoRepository _candidatoRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly IProcessoSelecaoRepository _processoRepository;
    private readonly IMapper _mapper;

    public InscricaoService(
        ICandidatoRepository candidatoRepository,
        IDocumentoRepository documentoRepository,
        IProcessoSelecaoRepository processoRepository,
        IMapper mapper)
    {
        _candidatoRepository = candidatoRepository;
        _documentoRepository = documentoRepository;
        _processoRepository = processoRepository;
        _mapper = mapper;
    }

    public async Task<InscricaoResultDto> CriarInscricaoCompletaAsync(CreateInscricaoCompletaDto dto, string caminhoBase)
    {
        try
        {
            var processo = await _processoRepository.GetByIdAsync(dto.ProcessoSelecaoId);
            if (processo == null)
                throw new Exception("Processo de seleção não encontrado");

            if (!processo.EstaDentroDoPrazo())
                throw new Exception("O prazo para este processo de seleção expirou");

            var email = dto.Pagina1?.Email;
            var cpf = dto.Pagina2?.Cpf;
            
            if (string.IsNullOrWhiteSpace(email))
                email = $"temp_{Guid.NewGuid()}@temp.com";
            
            if (string.IsNullOrWhiteSpace(cpf))
                cpf = Guid.NewGuid().ToString("N")[..11];

            var numeroInscricao = await GerarNumeroInscricaoAsync(dto.ProcessoSelecaoId);
            
            DateTime? dataNascimento = null;
            if (!string.IsNullOrEmpty(dto.Pagina1?.DataNascimento) && DateTime.TryParse(dto.Pagina1.DataNascimento, out var parsedData))
                dataNascimento = parsedData;
            
            DateTime? dataVencimentoRG = null;
            if (!string.IsNullOrEmpty(dto.Pagina2?.DataVencimentoRG) && DateTime.TryParse(dto.Pagina2.DataVencimentoRG, out var parsedVenc))
                dataVencimentoRG = parsedVenc;
            
            decimal? valorInscricao = null;
            if (!string.IsNullOrEmpty(dto.Pagina4?.ValorInscricao) && decimal.TryParse(dto.Pagina4.ValorInscricao, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var valorParsed))
                valorInscricao = valorParsed;
            
            var candidato = new Candidato
            {
                Nome = dto.Pagina1?.Nome ?? "Nome não informado",
                Email = email,
                Cpf = cpf,
                RG = dto.Pagina2?.NumeroRegistroGeral,
                Telefone = dto.Pagina1?.Telefone ?? dto.Pagina2?.Telefone1,
                AreaPesquisa = dto.Pagina1?.AreaOfertada ?? string.Empty,
                ProcessoSelecaoId = dto.ProcessoSelecaoId,
                NumeroInscricao = numeroInscricao,
                DataCadastro = DateTime.UtcNow,
                StatusValidacao = StatusValidacao.Pendente,
                DataNascimento = dataNascimento,
                PaisNatal = dto.Pagina2?.PaisNatal,
                EstadoNatal = dto.Pagina2?.EstadoNatal,
                Naturalidade = dto.Pagina2?.Naturalidade,
                NomeSocial = dto.Pagina2?.NomeSocial,
                EstadoCivil = dto.Pagina2?.EstadoCivil,
                Nacionalidade = dto.Pagina2?.Nacionalidade,
                Sexo = dto.Pagina2?.Sexo,
                Telefone2 = dto.Pagina2?.Telefone2,
                CorRaca = dto.Pagina2?.CorRaca,
                DataVencimentoRG = dataVencimentoRG,
                TipoVisto = dto.Pagina2?.TipoVisto,
                FormaInscricao = dto.Pagina4?.FormaInscricao,
                LocalProva = dto.Pagina4?.LocalProva,
                CampusProva = dto.Pagina4?.CampusProva,
                ValorInscricao = valorInscricao,
                DeficienciaFisica = dto.Pagina4?.DeficienciaFisica ?? false,
                DeficienciaAuditiva = dto.Pagina4?.DeficienciaAuditiva ?? false,
                DeficienciaFala = dto.Pagina4?.DeficienciaFala ?? false,
                DeficienciaVisual = dto.Pagina4?.DeficienciaVisual ?? false,
                DeficienciaMental = dto.Pagina4?.DeficienciaMental ?? false,
                DeficienciaIntelectual = dto.Pagina4?.DeficienciaIntelectual ?? false,
                DeficienciaReabilitado = dto.Pagina4?.DeficienciaReabilitado ?? false,
                DeficienciaMultipla = dto.Pagina4?.DeficienciaMultipla ?? false,
                MotivoOutrasNecessidades = dto.Pagina4?.MotivoOutrasNecessidades
            };

            var candidatoCriado = await _candidatoRepository.AddAsync(candidato);

            if (dto.Documentos != null)
            {
                foreach (var doc in dto.Documentos)
                {
                    if (doc.Arquivo != null && doc.Arquivo.Length > 0)
                    {
                        await SalvarDocumentoAsync(candidatoCriado.Id, doc.Tipo, doc.NomeArquivo, doc.Arquivo, caminhoBase);
                    }
                }
            }

            if (dto.DocumentosLink != null)
            {
                foreach (var doc in dto.DocumentosLink)
                {
                    if (!string.IsNullOrWhiteSpace(doc.LinkUrl))
                    {
                        await SalvarDocumentoLinkAsync(candidatoCriado.Id, doc.Tipo, doc.LinkUrl, doc.Descricao);
                    }
                }
            }

            return new InscricaoResultDto
            {
                CandidatoId = candidatoCriado.Id,
                ProcessoSelecaoId = dto.ProcessoSelecaoId,
                NumeroInscricao = numeroInscricao,
                Mensagem = "Inscrição realizada com sucesso!",
                DataInscricao = candidatoCriado.DataCadastro
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao criar inscrição: {ex.Message}", ex);
        }
    }

    private async Task SalvarDocumentoAsync(long candidatoId, TipoDocumento tipo, string nomeArquivo, byte[] arquivo, string caminhoBase)
    {
        var caminhoDiretorio = Path.Combine(caminhoBase, candidatoId.ToString());
        Directory.CreateDirectory(caminhoDiretorio);

        var nomeUnico = $"{Guid.NewGuid()}_{nomeArquivo}";
        var caminhoCompleto = Path.Combine(caminhoDiretorio, nomeUnico);

        await File.WriteAllBytesAsync(caminhoCompleto, arquivo);

        var hash = await CalcularHashAsync(caminhoCompleto);

        var documento = new Documento
        {
            Tipo = tipo,
            NomeArquivo = nomeArquivo,
            CaminhoLocal = caminhoCompleto,
            CandidatoId = candidatoId,
            DataUpload = DateTime.UtcNow,
            Validado = false,
            HashValidacao = hash
        };

        await _documentoRepository.AddAsync(documento);
    }

    private async Task SalvarDocumentoLinkAsync(long candidatoId, TipoDocumento tipo, string linkUrl, string? descricao)
    {
        var documento = new Documento
        {
            Tipo = tipo,
            NomeArquivo = descricao ?? "Currículo Lattes",
            LinkUrl = linkUrl,
            CaminhoLocal = string.Empty,
            CandidatoId = candidatoId,
            DataUpload = DateTime.UtcNow,
            Validado = false
        };

        await _documentoRepository.AddAsync(documento);
    }

    private static async Task<string> CalcularHashAsync(string filePath)
    {
        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = await sha256.ComputeHashAsync(stream);
        return Convert.ToHexString(hash);
    }

    private async Task<string> GerarNumeroInscricaoAsync(long processoSelecaoId)
    {
        var ano = DateTime.UtcNow.Year;
        var ultimoCandidato = await _candidatoRepository.GetLastByProcessoIdAsync(processoSelecaoId);
        
        int sequencial = 1;
        if (ultimoCandidato != null && ultimoCandidato.NumeroInscricao.StartsWith(ano.ToString()))
        {
            var parteSequencial = ultimoCandidato.NumeroInscricao.Substring(4);
            if (int.TryParse(parteSequencial, out var ultimoNumero))
            {
                sequencial = ultimoNumero + 1;
            }
        }
        
        return $"{ano}{processoSelecaoId:D3}{sequencial:D5}";
    }
}

public class InscricaoResultDto
{
    public long CandidatoId { get; set; }
    public long ProcessoSelecaoId { get; set; }
    public string NumeroInscricao { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public DateTime DataInscricao { get; set; }
}

public class CreateInscricaoCompletaDto
{
    public long ProcessoSelecaoId { get; set; }
    public Pagina1Dto? Pagina1 { get; set; }
    public Pagina2Dto? Pagina2 { get; set; }
    public Pagina4Dto? Pagina4 { get; set; }
    public List<DocumentoUploadDto>? Documentos { get; set; }
    public List<DocumentoLinkDto>? DocumentosLink { get; set; }
}

public class DocumentoUploadDto
{
    public TipoDocumento Tipo { get; set; }
    public string NomeArquivo { get; set; } = string.Empty;
    public byte[]? Arquivo { get; set; }
}

public class DocumentoLinkDto
{
    public TipoDocumento Tipo { get; set; }
    public string LinkUrl { get; set; } = string.Empty;
    public string? Descricao { get; set; }
}
