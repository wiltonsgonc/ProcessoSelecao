using System.Text.Json;
using AutoMapper;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Application.Services;

/// <summary>
/// Interface do serviço de Baremas/Avaliações
/// </summary>
public interface IBaremaService
{
    /// <summary>Retorna todos os baremas</summary>
    Task<IEnumerable<BaremaDto>> GetAllAsync();
    
    /// <summary>Retorna um barema pelo ID</summary>
    Task<BaremaDto?> GetByIdAsync(long id);
    
    /// <summary>Cria um novo barema</summary>
    Task<BaremaDto> CreateAsync(CreateBaremaDto dto);
    
    /// <summary>Atualiza critérios de um barema</summary>
    Task<BaremaDto> UpdateCriteriosAsync(long id, UpdateBaremaDto dto);
    
    /// <summary>Finaliza um barema</summary>
    Task<BaremaDto> FinalizarAsync(long id, FinalizarBaremaDto dto);
    
    /// <summary>Remove um barema</summary>
    Task DeleteAsync(long id);
    
    /// <summary>Retorna baremas de um candidato</summary>
    Task<IEnumerable<BaremaDto>> GetByCandidatoIdAsync(long candidatoId);
    
    /// <summary>Retorna baremas de um avaliador</summary>
    Task<IEnumerable<BaremaDto>> GetByAvaliadorIdAsync(long avaliadorId);
}

/// <summary>
/// Serviço para manipulação de Baremas/Avaliações
/// </summary>
public class BaremaService : IBaremaService
{
    private readonly IBaremaRepository _repository;
    private readonly IMapper _mapper;

    public BaremaService(IBaremaRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <summary>Retorna todos os baremas</summary>
    public async Task<IEnumerable<BaremaDto>> GetAllAsync()
    {
        var baremas = await _repository.GetAllAsync();
        return baremas.Select(MapToDto);
    }

    /// <summary>Retorna um barema pelo ID</summary>
    public async Task<BaremaDto?> GetByIdAsync(long id)
    {
        var barema = await _repository.GetByIdAsync(id);
        return barema != null ? MapToDto(barema) : null;
    }

    /// <summary>Cria um novo barema</summary>
    public async Task<BaremaDto> CreateAsync(CreateBaremaDto dto)
    {
        var entity = new Barema
        {
            CandidatoId = dto.CandidatoId,
            AvaliadorId = dto.AvaliadorId,
            Status = StatusBarema.Pendente
        };
        var created = await _repository.AddAsync(entity);
        return MapToDto(created);
    }

    /// <summary>Atualiza critérios de um barema</summary>
    public async Task<BaremaDto> UpdateCriteriosAsync(long id, UpdateBaremaDto dto)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new Exception("Barema não encontrado");
        
        if (entity.Status == StatusBarema.Concluido)
            throw new Exception("Barema já foi finalizado");
        
        entity.CriteriosJson = JsonSerializer.Serialize(dto.Criterios);
        entity.Observacoes = dto.Observacoes;
        entity.Status = StatusBarema.EmPreenchimento;
        
        var updated = await _repository.UpdateAsync(entity);
        return MapToDto(updated);
    }

    /// <summary>Finaliza um barema</summary>
    public async Task<BaremaDto> FinalizarAsync(long id, FinalizarBaremaDto dto)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new Exception("Barema não encontrado");
        
        entity.CriteriosJson = JsonSerializer.Serialize(dto.Criterios);
        entity.NotaFinal = entity.CalcularNotaFinal(dto.Criterios);
        entity.Observacoes = dto.Observacoes;
        entity.DataPreenchimento = DateTime.UtcNow;
        entity.Status = StatusBarema.Concluido;
        
        var updated = await _repository.UpdateAsync(entity);
        return MapToDto(updated);
    }

    /// <summary>Remove um barema</summary>
    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
    }

    /// <summary>Retorna baremas de um candidato</summary>
    public async Task<IEnumerable<BaremaDto>> GetByCandidatoIdAsync(long candidatoId)
    {
        var baremas = await _repository.GetByCandidatoIdAsync(candidatoId);
        return baremas.Select(MapToDto);
    }

    /// <summary>Retorna baremas de um avaliador</summary>
    public async Task<IEnumerable<BaremaDto>> GetByAvaliadorIdAsync(long avaliadorId)
    {
        var baremas = await _repository.GetByAvaliadorIdAsync(avaliadorId);
        return baremas.Select(MapToDto);
    }

    private BaremaDto MapToDto(Barema barema)
    {
        var dto = _mapper.Map<BaremaDto>(barema);
        dto.CandidatoNome = barema.Candidato?.Nome;
        dto.AvaliadorNome = barema.Avaliador?.Nome;
        
        if (!string.IsNullOrEmpty(barema.CriteriosJson))
        {
            dto.Criterios = JsonSerializer.Deserialize<Dictionary<string, float>>(barema.CriteriosJson);
        }
        
        return dto;
    }
}
