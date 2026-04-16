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

            var candidato = new Candidato
            {
                Nome = dto.Pagina1?.Nome ?? "Nome não informado",
                Email = email,
                Cpf = cpf,
                RG = dto.Pagina2?.NumeroRegistroGeral,
                Telefone = dto.Pagina1?.Telefone ?? dto.Pagina2?.Telefone1,
                AreaPesquisa = dto.Pagina1?.AreaOfertada ?? string.Empty,
                ProcessoSelecaoId = dto.ProcessoSelecaoId,
                DataCadastro = DateTime.UtcNow,
                StatusValidacao = StatusValidacao.Pendente
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

            return new InscricaoResultDto
            {
                CandidatoId = candidatoCriado.Id,
                ProcessoSelecaoId = dto.ProcessoSelecaoId,
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

    private static async Task<string> CalcularHashAsync(string filePath)
    {
        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = await sha256.ComputeHashAsync(stream);
        return Convert.ToHexString(hash);
    }
}

public class InscricaoResultDto
{
    public long CandidatoId { get; set; }
    public long ProcessoSelecaoId { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public DateTime DataInscricao { get; set; }
}

public class CreateInscricaoCompletaDto
{
    public long ProcessoSelecaoId { get; set; }
    public Pagina1Dto? Pagina1 { get; set; }
    public Pagina2Dto? Pagina2 { get; set; }
    public List<DocumentoUploadDto>? Documentos { get; set; }
}

public class DocumentoUploadDto
{
    public TipoDocumento Tipo { get; set; }
    public string NomeArquivo { get; set; } = string.Empty;
    public byte[]? Arquivo { get; set; }
}
