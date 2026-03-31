using AutoMapper;
using Microsoft.Extensions.Configuration;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Application.Services;

/// <summary>
/// Interface do serviço de Documentos
/// </summary>
public interface IDocumentoService
{
    /// <summary>Retorna todos os documentos</summary>
    Task<IEnumerable<DocumentoDto>> GetAllAsync();
    
    /// <summary>Retorna um documento pelo ID</summary>
    Task<DocumentoDto?> GetByIdAsync(long id);
    
    /// <summary>Cria um novo documento com upload de arquivo</summary>
    Task<DocumentoDto> CreateAsync(CreateDocumentoDto dto, Stream fileStream, string caminhoBase);
    
    /// <summary>Valida um documento</summary>
    Task<DocumentoDto> ValidateAsync(long id, ValidateDocumentoDto dto);
    
    /// <summary>Remove um documento</summary>
    Task DeleteAsync(long id);
    
    /// <summary>Retorna documentos de um candidato</summary>
    Task<IEnumerable<DocumentoDto>> GetByCandidatoIdAsync(long candidatoId);
    
    /// <summary>Retorna o caminho do arquivo</summary>
    Task<string> GetFilePathAsync(long id);
}

/// <summary>
/// Serviço para manipulação de Documentos
/// </summary>
public class DocumentoService : IDocumentoService
{
    private readonly IDocumentoRepository _repository;
    private readonly IMapper _mapper;
    private readonly string _caminhoBase;

    public DocumentoService(IDocumentoRepository repository, IMapper mapper, IConfiguration configuration)
    {
        _repository = repository;
        _mapper = mapper;
        _caminhoBase = configuration["Storage:CaminhoBase"] ?? "/app/documentos";
    }

    /// <summary>Retorna todos os documentos</summary>
    public async Task<IEnumerable<DocumentoDto>> GetAllAsync()
    {
        var documentos = await _repository.GetAllAsync();
        var dtos = _mapper.Map<IEnumerable<DocumentoDto>>(documentos);
        foreach (var dto in dtos)
        {
            var doc = documentos.FirstOrDefault(d => d.Id == dto.Id);
            dto.CandidatoNome = doc?.Candidato?.Nome;
        }
        return dtos;
    }

    /// <summary>Retorna um documento pelo ID</summary>
    public async Task<DocumentoDto?> GetByIdAsync(long id)
    {
        var documento = await _repository.GetByIdAsync(id);
        return documento != null ? _mapper.Map<DocumentoDto>(documento) : null;
    }

    /// <summary>Cria um novo documento com upload de arquivo</summary>
    public async Task<DocumentoDto> CreateAsync(CreateDocumentoDto dto, Stream fileStream, string caminhoBase)
    {
        var caminhoDiretorio = Path.Combine(caminhoBase, dto.CandidatoId.ToString());
        Directory.CreateDirectory(caminhoDiretorio);
        
        var nomeUnico = $"{Guid.NewGuid()}_{dto.NomeArquivo}";
        var caminhoCompleto = Path.Combine(caminhoDiretorio, nomeUnico);
        
        using (var fs = new FileStream(caminhoCompleto, FileMode.Create))
        {
            await fileStream.CopyToAsync(fs);
        }

        var entity = new Documento
        {
            Tipo = dto.Tipo,
            NomeArquivo = dto.NomeArquivo,
            CaminhoLocal = caminhoCompleto,
            CandidatoId = dto.CandidatoId,
            DataUpload = DateTime.UtcNow,
            Validado = false,
            HashValidacao = await CalculateHashAsync(caminhoCompleto)
        };

        var created = await _repository.AddAsync(entity);
        return _mapper.Map<DocumentoDto>(created);
    }

    /// <summary>Valida um documento</summary>
    public async Task<DocumentoDto> ValidateAsync(long id, ValidateDocumentoDto dto)
    {
        var documento = await _repository.GetByIdAsync(id) ?? throw new Exception("Documento não encontrado");
        documento.Validado = dto.Validado;
        documento.MotivoRejeicao = dto.MotivoRejeicao;
        var updated = await _repository.UpdateAsync(documento);
        return _mapper.Map<DocumentoDto>(updated);
    }

    /// <summary>Remove um documento</summary>
    public async Task DeleteAsync(long id)
    {
        var documento = await _repository.GetByIdAsync(id);
        if (documento != null && File.Exists(documento.CaminhoLocal))
        {
            File.Delete(documento.CaminhoLocal);
        }
        await _repository.DeleteAsync(id);
    }

    /// <summary>Retorna documentos de um candidato</summary>
    public async Task<IEnumerable<DocumentoDto>> GetByCandidatoIdAsync(long candidatoId)
    {
        var documentos = await _repository.GetByCandidatoIdAsync(candidatoId);
        return _mapper.Map<IEnumerable<DocumentoDto>>(documentos);
    }

    /// <summary>Retorna o caminho do arquivo</summary>
    public async Task<string> GetFilePathAsync(long id)
    {
        var documento = await _repository.GetByIdAsync(id) ?? throw new Exception("Documento não encontrado");
        
        var caminhoArquivo = documento.CaminhoLocal;
        
        // Se o caminho já for um caminho absoluto válido dentro do container, usa diretamente
        if (caminhoArquivo.StartsWith("/app/") || caminhoArquivo.StartsWith("/"))
        {
            if (File.Exists(caminhoArquivo))
            {
                return caminhoArquivo;
            }
            // Tenta mapear para o caminho base
            var nomeArquivo = Path.GetFileName(caminhoArquivo);
            caminhoArquivo = Path.Combine(_caminhoBase, nomeArquivo);
        }
        // Se o caminho for relativo (começa com ./ ou não for um caminho absoluto), combina com o caminho base
        else if (!Path.IsPathRooted(caminhoArquivo) || caminhoArquivo.StartsWith("."))
        {
            // Remove o prefixo ./ se existir
            if (caminhoArquivo.StartsWith("./"))
            {
                caminhoArquivo = caminhoArquivo.Substring(2);
            }
            // Se o caminho já contém "documentos/", não adiciona novamente
            if (!caminhoArquivo.StartsWith("documentos/"))
            {
                caminhoArquivo = Path.Combine("documentos", caminhoArquivo);
            }
            caminhoArquivo = Path.Combine(_caminhoBase, caminhoArquivo);
        }
        
        if (!File.Exists(caminhoArquivo))
        {
            throw new FileNotFoundException($"Arquivo não encontrado: {caminhoArquivo}");
        }
        return caminhoArquivo;
    }

    private static async Task<string> CalculateHashAsync(string filePath)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = await sha256.ComputeHashAsync(stream);
        return Convert.ToHexString(hash);
    }
}
