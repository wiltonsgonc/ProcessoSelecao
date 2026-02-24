using System.Text.Json;
using AutoMapper;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Application.Services;

public interface IBaremaService
{
    Task<IEnumerable<BaremaDto>> GetAllAsync();
    Task<BaremaDto?> GetByIdAsync(long id);
    Task<BaremaDto> CreateAsync(CreateBaremaDto dto);
    Task<BaremaDto> UpdateCriteriosAsync(long id, UpdateBaremaDto dto);
    Task<BaremaDto> FinalizarAsync(long id, FinalizarBaremaDto dto);
    Task DeleteAsync(long id);
    Task<IEnumerable<BaremaDto>> GetByCandidatoIdAsync(long candidatoId);
    Task<IEnumerable<BaremaDto>> GetByAvaliadorIdAsync(long avaliadorId);
}

public class BaremaService : IBaremaService
{
    private readonly IBaremaRepository _repository;
    private readonly IMapper _mapper;

    public BaremaService(IBaremaRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BaremaDto>> GetAllAsync()
    {
        var baremas = await _repository.GetAllAsync();
        return baremas.Select(MapToDto);
    }

    public async Task<BaremaDto?> GetByIdAsync(long id)
    {
        var barema = await _repository.GetByIdAsync(id);
        return barema != null ? MapToDto(barema) : null;
    }

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

    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<BaremaDto>> GetByCandidatoIdAsync(long candidatoId)
    {
        var baremas = await _repository.GetByCandidatoIdAsync(candidatoId);
        return baremas.Select(MapToDto);
    }

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
