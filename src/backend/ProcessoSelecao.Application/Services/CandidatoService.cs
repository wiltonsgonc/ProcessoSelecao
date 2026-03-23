using AutoMapper;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Application.Services;

/// <summary>
/// Interface do serviço de Candidatos
/// </summary>
public interface ICandidatoService
{
    /// <summary>Retorna todos os candidatos</summary>
    Task<IEnumerable<CandidatoDto>> GetAllAsync();
    
    /// <summary>Retorna um candidato pelo ID</summary>
    Task<CandidatoDto?> GetByIdAsync(long id);
    
    /// <summary>Cria um novo candidato</summary>
    Task<CandidatoDto> CreateAsync(CreateCandidatoDto dto);
    
    /// <summary>Atualiza um candidato</summary>
    Task<CandidatoDto> UpdateAsync(long id, UpdateCandidatoDto dto);
    
    /// <summary>Remove um candidato</summary>
    Task DeleteAsync(long id);
    
    /// <summary>Retorna candidatos de um processo</summary>
    Task<IEnumerable<CandidatoDto>> GetByProcessoIdAsync(long processoId);
    
    /// <summary>Retorna a pontuação de um candidato</summary>
    Task<float> GetPontuacaoAsync(long id);
}

/// <summary>
/// Serviço para manipulação de Candidatos
/// </summary>
public class CandidatoService : ICandidatoService
{
    private readonly ICandidatoRepository _repository;
    private readonly IMapper _mapper;

    public CandidatoService(ICandidatoRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <summary>Retorna todos os candidatos</summary>
    public async Task<IEnumerable<CandidatoDto>> GetAllAsync()
    {
        var candidatos = await _repository.GetAllAsync();
        return candidatos.Select(c => MapToDto(c));
    }

    /// <summary>Retorna um candidato pelo ID</summary>
    public async Task<CandidatoDto?> GetByIdAsync(long id)
    {
        var candidato = await _repository.GetByIdAsync(id);
        return candidato != null ? MapToDto(candidato) : null;
    }

    /// <summary>Cria um novo candidato</summary>
    public async Task<CandidatoDto> CreateAsync(CreateCandidatoDto dto)
    {
        var entity = _mapper.Map<Candidato>(dto);
        entity.DataCadastro = DateTime.UtcNow;
        entity.StatusValidacao = StatusValidacao.Pendente;
        var created = await _repository.AddAsync(entity);
        return MapToDto(created);
    }

    /// <summary>Atualiza um candidato</summary>
    public async Task<CandidatoDto> UpdateAsync(long id, UpdateCandidatoDto dto)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new Exception("Candidato não encontrado");
        _mapper.Map(dto, entity);
        var updated = await _repository.UpdateAsync(entity);
        return MapToDto(updated);
    }

    /// <summary>Remove um candidato</summary>
    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
    }

    /// <summary>Retorna candidatos de um processo</summary>
    public async Task<IEnumerable<CandidatoDto>> GetByProcessoIdAsync(long processoId)
    {
        var candidatos = await _repository.GetByProcessoIdAsync(processoId);
        return candidatos.Select(c => MapToDto(c));
    }

    /// <summary>Retorna a pontuação de um candidato</summary>
    public async Task<float> GetPontuacaoAsync(long id)
    {
        var candidato = await _repository.GetByIdAsync(id) ?? throw new Exception("Candidato não encontrado");
        return candidato.CalcularPontuacao();
    }

    private CandidatoDto MapToDto(Candidato candidato)
    {
        var dto = _mapper.Map<CandidatoDto>(candidato);
        dto.PontuacaoMedia = candidato.CalcularPontuacao();
        dto.TotalDocumentos = candidato.Documentos.Count;
        dto.DocumentosValidados = candidato.Documentos.Count(d => d.Validado);
        return dto;
    }
}
