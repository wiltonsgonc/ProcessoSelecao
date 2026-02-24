using AutoMapper;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Application.Services;

public interface IAvaliadorService
{
    Task<IEnumerable<AvaliadorDto>> GetAllAsync();
    Task<AvaliadorDto?> GetByIdAsync(long id);
    Task<AvaliadorDto> CreateAsync(CreateAvaliadorDto dto);
    Task<AvaliadorDto> UpdateAsync(long id, UpdateAvaliadorDto dto);
    Task DeleteAsync(long id);
    Task<IEnumerable<AvaliadorDto>> GetByProcessoIdAsync(long processoId);
}

public class AvaliadorService : IAvaliadorService
{
    private readonly IAvaliadorRepository _repository;
    private readonly IMapper _mapper;

    public AvaliadorService(IAvaliadorRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AvaliadorDto>> GetAllAsync()
    {
        var avaliadores = await _repository.GetAllAsync();
        return avaliadores.Select(MapToDto);
    }

    public async Task<AvaliadorDto?> GetByIdAsync(long id)
    {
        var avaliador = await _repository.GetByIdAsync(id);
        return avaliador != null ? MapToDto(avaliador) : null;
    }

    public async Task<AvaliadorDto> CreateAsync(CreateAvaliadorDto dto)
    {
        var entity = _mapper.Map<Avaliador>(dto);
        entity.Ativo = true;
        var created = await _repository.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<AvaliadorDto> UpdateAsync(long id, UpdateAvaliadorDto dto)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new Exception("Avaliador não encontrado");
        _mapper.Map(dto, entity);
        var updated = await _repository.UpdateAsync(entity);
        return MapToDto(updated);
    }

    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
    }

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
