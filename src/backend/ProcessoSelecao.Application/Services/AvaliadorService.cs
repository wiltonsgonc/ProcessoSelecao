using AutoMapper;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Application.Services;

/// <summary>
/// Interface do serviço de Avaliadores
/// </summary>
public interface IAvaliadorService
{
    /// <summary>Retorna todos os avaliadores</summary>
    Task<IEnumerable<AvaliadorDto>> GetAllAsync();
    
    /// <summary>Retorna um avaliador pelo ID</summary>
    Task<AvaliadorDto?> GetByIdAsync(long id);
    
    /// <summary>Cria um novo avaliador</summary>
    Task<AvaliadorDto> CreateAsync(CreateAvaliadorDto dto);
    
    /// <summary>Atualiza um avaliador</summary>
    Task<AvaliadorDto> UpdateAsync(long id, UpdateAvaliadorDto dto);
    
    /// <summary>Remove um avaliador</summary>
    Task DeleteAsync(long id);
    
    /// <summary>Retorna avaliadores de um processo</summary>
    Task<IEnumerable<AvaliadorDto>> GetByProcessoIdAsync(long processoId);
}

/// <summary>
/// Serviço para manipulação de Avaliadores
/// </summary>
public class AvaliadorService : IAvaliadorService
{
    private readonly IAvaliadorRepository _repository;
    private readonly IMapper _mapper;

    public AvaliadorService(IAvaliadorRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <summary>Retorna todos os avaliadores</summary>
    public async Task<IEnumerable<AvaliadorDto>> GetAllAsync()
    {
        var avaliadores = await _repository.GetAllAsync();
        return avaliadores.Select(MapToDto);
    }

    /// <summary>Retorna um avaliador pelo ID</summary>
    public async Task<AvaliadorDto?> GetByIdAsync(long id)
    {
        var avaliador = await _repository.GetByIdAsync(id);
        return avaliador != null ? MapToDto(avaliador) : null;
    }

    /// <summary>Cria um novo avaliador</summary>
    public async Task<AvaliadorDto> CreateAsync(CreateAvaliadorDto dto)
    {
        var entity = _mapper.Map<Avaliador>(dto);
        entity.Ativo = true;
        var created = await _repository.AddAsync(entity);
        return MapToDto(created);
    }

    /// <summary>Atualiza um avaliador</summary>
    public async Task<AvaliadorDto> UpdateAsync(long id, UpdateAvaliadorDto dto)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new Exception("Avaliador não encontrado");
        _mapper.Map(dto, entity);
        var updated = await _repository.UpdateAsync(entity);
        return MapToDto(updated);
    }

    /// <summary>Remove um avaliador</summary>
    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
    }

    /// <summary>Retorna avaliadores de um processo</summary>
    public async Task<IEnumerable<AvaliadorDto>> GetByProcessoIdAsync(long processoId)
    {
        var avaliadores = await _repository.GetByProcessoIdAsync(processoId);
        return avaliadores.Select(MapToDto);
    }

    private AvaliadorDto MapToDto(Avaliador avaliador)
    {
        var dto = _mapper.Map<AvaliadorDto>(avaliador);
        dto.AvaliacoesPendentes = avaliador.Baremas?.Count(b => b.Status == Domain.Enums.StatusBarema.Pendente) ?? 0;
        return dto;
    }
}
