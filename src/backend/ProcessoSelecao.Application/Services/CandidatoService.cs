using AutoMapper;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Application.Services;

public interface ICandidatoService
{
    Task<IEnumerable<CandidatoDto>> GetAllAsync();
    Task<CandidatoDto?> GetByIdAsync(long id);
    Task<CandidatoDto> CreateAsync(CreateCandidatoDto dto);
    Task<CandidatoDto> UpdateAsync(long id, UpdateCandidatoDto dto);
    Task DeleteAsync(long id);
    Task<IEnumerable<CandidatoDto>> GetByProcessoIdAsync(long processoId);
    Task<float> GetPontuacaoAsync(long id);
}

public class CandidatoService : ICandidatoService
{
    private readonly ICandidatoRepository _repository;
    private readonly IMapper _mapper;

    public CandidatoService(ICandidatoRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CandidatoDto>> GetAllAsync()
    {
        var candidatos = await _repository.GetAllAsync();
        return candidatos.Select(c => MapToDto(c));
    }

    public async Task<CandidatoDto?> GetByIdAsync(long id)
    {
        var candidato = await _repository.GetByIdAsync(id);
        return candidato != null ? MapToDto(candidato) : null;
    }

    public async Task<CandidatoDto> CreateAsync(CreateCandidatoDto dto)
    {
        var entity = _mapper.Map<Candidato>(dto);
        entity.DataCadastro = DateTime.UtcNow;
        entity.StatusValidacao = StatusValidacao.Pendente;
        var created = await _repository.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<CandidatoDto> UpdateAsync(long id, UpdateCandidatoDto dto)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new Exception("Candidato não encontrado");
        _mapper.Map(dto, entity);
        var updated = await _repository.UpdateAsync(entity);
        return MapToDto(updated);
    }

    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<CandidatoDto>> GetByProcessoIdAsync(long processoId)
    {
        var candidatos = await _repository.GetByProcessoIdAsync(processoId);
        return candidatos.Select(c => MapToDto(c));
    }

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
