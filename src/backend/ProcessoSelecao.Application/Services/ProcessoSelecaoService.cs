using AutoMapper;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Application.Services;

/// <summary>
/// Interface do serviço de Processos de Seleção
/// </summary>
public interface IProcessoSelecaoService
{
    /// <summary>Retorna todos os processos</summary>
    Task<IEnumerable<ProcessoSelecaoDto>> GetAllAsync();
    
    /// <summary>Retorna um processo pelo ID</summary>
    Task<ProcessoSelecaoDto?> GetByIdAsync(long id);
    
    /// <summary>Cria um novo processo</summary>
    Task<ProcessoSelecaoDto> CreateAsync(CreateProcessoSelecaoDto dto);
    
    /// <summary>Atualiza um processo</summary>
    Task<ProcessoSelecaoDto> UpdateAsync(long id, UpdateProcessoSelecaoDto dto);
    
    /// <summary>Remove um processo</summary>
    Task DeleteAsync(long id);
    
    /// <summary>Inicia um processo</summary>
    Task<ProcessoSelecaoDto> IniciarAsync(long id);
    
    /// <summary>Finaliza um processo</summary>
    Task<ProcessoSelecaoDto> FinalizarAsync(long id);
}

/// <summary>
/// Serviço para manipulação de Processos de Seleção
/// </summary>
public class ProcessoSelecaoService : IProcessoSelecaoService
{
    private readonly IProcessoSelecaoRepository _repository;
    private readonly IMapper _mapper;

    public ProcessoSelecaoService(IProcessoSelecaoRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <summary>Retorna todos os processos</summary>
    public async Task<IEnumerable<ProcessoSelecaoDto>> GetAllAsync()
    {
        var processos = await _repository.GetAllAsync();
        return processos.Select(MapToDto);
    }

    /// <summary>Retorna um processo pelo ID</summary>
    public async Task<ProcessoSelecaoDto?> GetByIdAsync(long id)
    {
        var processo = await _repository.GetByIdAsync(id);
        return processo != null ? MapToDto(processo) : null;
    }

    /// <summary>Cria um novo processo</summary>
    public async Task<ProcessoSelecaoDto> CreateAsync(CreateProcessoSelecaoDto dto)
    {
        var entity = _mapper.Map<Domain.Entities.ProcessoSelecao>(dto);
        entity.Status = StatusProcesso.Rascunho;
        var created = await _repository.AddAsync(entity);
        return MapToDto(created);
    }

    /// <summary>Atualiza um processo</summary>
    public async Task<ProcessoSelecaoDto> UpdateAsync(long id, UpdateProcessoSelecaoDto dto)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new Exception("Processo não encontrado");
        
        if (entity.Status == StatusProcesso.Finalizado)
            throw new Exception("Processo já finalizado não pode ser alterado");
        
        _mapper.Map(dto, entity);
        var updated = await _repository.UpdateAsync(entity);
        return MapToDto(updated);
    }

    /// <summary>Remove um processo</summary>
    public async Task DeleteAsync(long id)
    {
        var processo = await _repository.GetByIdAsync(id) ?? throw new Exception("Processo não encontrado");
        if (processo.Status == StatusProcesso.Finalizado)
        {
            throw new Exception("Não é possível excluir um processo finalizados");
        }
        await _repository.DeleteAsync(id);
    }

    /// <summary>Inicia um processo</summary>
    public async Task<ProcessoSelecaoDto> IniciarAsync(long id)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new Exception("Processo não encontrado");
        entity.IniciarProcesso();
        var updated = await _repository.UpdateAsync(entity);
        return MapToDto(updated);
    }

    /// <summary>Finaliza um processo</summary>
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
