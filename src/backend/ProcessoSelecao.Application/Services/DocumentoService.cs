using AutoMapper;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Application.Services;

public interface IDocumentoService
{
    Task<IEnumerable<DocumentoDto>> GetAllAsync();
    Task<DocumentoDto?> GetByIdAsync(long id);
    Task<DocumentoDto> CreateAsync(CreateDocumentoDto dto, Stream fileStream, string caminhoBase);
    Task<DocumentoDto> ValidateAsync(long id, ValidateDocumentoDto dto);
    Task DeleteAsync(long id);
    Task<IEnumerable<DocumentoDto>> GetByCandidatoIdAsync(long candidatoId);
    Task<string> GetFilePathAsync(long id);
}

public class DocumentoService : IDocumentoService
{
    private readonly IDocumentoRepository _repository;
    private readonly IMapper _mapper;

    public DocumentoService(IDocumentoRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DocumentoDto>> GetAllAsync()
    {
        var documentos = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<DocumentoDto>>(documentos);
    }

    public async Task<DocumentoDto?> GetByIdAsync(long id)
    {
        var documento = await _repository.GetByIdAsync(id);
        return documento != null ? _mapper.Map<DocumentoDto>(documento) : null;
    }

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

    public async Task<DocumentoDto> ValidateAsync(long id, ValidateDocumentoDto dto)
    {
        var documento = await _repository.GetByIdAsync(id) ?? throw new Exception("Documento não encontrado");
        documento.Validado = dto.Validado;
        documento.MotivoRejeicao = dto.MotivoRejeicao;
        var updated = await _repository.UpdateAsync(documento);
        return _mapper.Map<DocumentoDto>(updated);
    }

    public async Task DeleteAsync(long id)
    {
        var documento = await _repository.GetByIdAsync(id);
        if (documento != null && File.Exists(documento.CaminhoLocal))
        {
            File.Delete(documento.CaminhoLocal);
        }
        await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<DocumentoDto>> GetByCandidatoIdAsync(long candidatoId)
    {
        var documentos = await _repository.GetByCandidatoIdAsync(candidatoId);
        return _mapper.Map<IEnumerable<DocumentoDto>>(documentos);
    }

    public async Task<string> GetFilePathAsync(long id)
    {
        var documento = await _repository.GetByIdAsync(id) ?? throw new Exception("Documento não encontrado");
        if (!File.Exists(documento.CaminhoLocal))
        {
            throw new FileNotFoundException("Arquivo não encontrado");
        }
        return documento.CaminhoLocal;
    }

    private static async Task<string> CalculateHashAsync(string filePath)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = await sha256.ComputeHashAsync(stream);
        return Convert.ToHexString(hash);
    }
}
