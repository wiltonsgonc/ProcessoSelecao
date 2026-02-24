using AutoMapper;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Application.Services;

public interface IProcessoSelecaoService
{
    Task<IEnumerable<ProcessoSelecaoDto>> GetAllAsync();
    Task<ProcessoSelecaoDto?> GetByIdAsync(long id);
    Task<ProcessoSelecaoDto> CreateAsync(CreateProcessoSelecaoDto dto);
    Task<ProcessoSelecaoDto> UpdateAsync(long id, UpdateProcessoSelecaoDto dto);
    Task DeleteAsync(long id);
    Task<ProcessoSelecaoDto> IniciarAsync(long id);
    Task<ProcessoSelecaoDto> FinalizarAsync(long id);
}

public class ProcessoSelecaoService : IProcessoSelecaoService
{
    private readonly IProcessoSelecaoRepository _repository;
    private readonly IMapper _mapper;

    public ProcessoSelecaoService(IProcessoSelecaoRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProcessoSelecaoDto>> GetAllAsync()
    {
        var processos = await _repository.GetAllAsync();
        return processos.Select(MapToDto);
    }

    public async Task<ProcessoSelecaoDto?> GetByIdAsync(long id)
    {
        var processo = await _repository.GetByIdAsync(id);
        return processo != null ? MapToDto(processo) : null;
    }

    public async Task<ProcessoSelecaoDto> CreateAsync(CreateProcessoSelecaoDto dto)
    {
        var entity = _mapper.Map<Domain.Entities.ProcessoSelecao>(dto);
        entity.Status = StatusProcesso.Rascunho;
        var created = await _repository.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<ProcessoSelecaoDto> UpdateAsync(long id, UpdateProcessoSelecaoDto dto)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new Exception("Processo não encontrado");
        
        if (entity.Status == StatusProcesso.Finalizado)
            throw new Exception("Processo já finalizado não pode ser alterado");
        
        _mapper.Map(dto, entity);
        var updated = await _repository.UpdateAsync(entity);
        return MapToDto(updated);
    }

    public async Task DeleteAsync(long id)
    {
        var processo = await _repository.GetByIdAsync(id);
        if (processo != null && processo.Status != StatusProcesso.Finalizado)
        {
            await _repository.DeleteAsync(id);
        }
    }

    public async Task<ProcessoSelecaoDto> IniciarAsync(long id)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new Exception("Processo não encontrado");
        entity.IniciarProcesso();
        var updated = await _repository.UpdateAsync(entity);
        return MapToDto(updated);
    }

    public async Task<ProcessoSelecaoDto> FinalizarAsync(long id)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new Exception("Processo não encontrado");
        entity.FinalizarProcesso();
        var updated = await _repository.UpdateAsync(entity);
        return MapToDto(updated);
    }

    private ProcessoSelecaoDto MapToDto(Domain.Entities.ProcessoSelecao processo)
    {
        var dto = _mapper.Map<ProcessoSelecaoDto>(processo);
        dto.TotalCandidatos = processo.Candidatos?.Count ?? 0;
        dto.TotalAvaliadores = processo.Avaliadores?.Count ?? 0;
        return dto;
    }
}
