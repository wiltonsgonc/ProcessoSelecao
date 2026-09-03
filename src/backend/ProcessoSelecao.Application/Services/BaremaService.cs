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
    
    /// <summary>Retorna dados para preenchimento automático do barema</summary>
    Task<BaremaDadosDto?> GetDadosBaremaAsync(long baremaId);
    
    /// <summary>Finaliza automaticamente por eliminação na análise documental</summary>
    Task<BaremaDto> FinalizarPorEliminacaoAsync(long baremaId);
    
    /// <summary>Retorna progresso de avaliação por candidato em um processo</summary>
    Task<IEnumerable<ProgressoCandidatoDto>> GetProgressoAsync(long processoId);
    
    /// <summary>Retorna progresso de avaliação de todos os processos</summary>
    Task<IEnumerable<ProgressoCandidatoDto>> GetProgressoAsync();
}

/// <summary>
/// Serviço para manipulação de Baremas/Avaliações
/// </summary>
public class BaremaService : IBaremaService
{
    private readonly IBaremaRepository _repository;
    private readonly ICandidatoRepository _candidatoRepository;
    private readonly IAvaliadorRepository _avaliadorRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly IMapper _mapper;

    public BaremaService(IBaremaRepository repository, ICandidatoRepository candidatoRepository, IAvaliadorRepository avaliadorRepository, IDocumentoRepository documentoRepository, IMapper mapper)
    {
        _repository = repository;
        _candidatoRepository = candidatoRepository;
        _avaliadorRepository = avaliadorRepository;
        _documentoRepository = documentoRepository;
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
        var candidato = await _candidatoRepository.GetByIdAsync(dto.CandidatoId);
        if (candidato == null)
            throw new Exception("Candidato não encontrado");

        var avaliador = await _avaliadorRepository.GetByIdAsync(dto.AvaliadorId);
        if (avaliador == null)
            throw new Exception("Avaliador não encontrado");

        if (!string.IsNullOrEmpty(candidato.Orientador) &&
            !string.IsNullOrEmpty(avaliador.Nome) &&
            candidato.Orientador.Equals(avaliador.Nome, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("O avaliador não pode avaliar um candidato que ele é o orientador");
        }

        var entity = new Barema
        {
            CandidatoId = dto.CandidatoId,
            AvaliadorId = dto.AvaliadorId,
            TipoBarema = dto.TipoBarema,
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
        
        if (entity.TipoBarema == "PIBIC")
        {
            entity.NotaFinal = entity.CalcularNotaFinalPibic(entity.CriteriosJson);
        }
        else
        {
            entity.NotaFinal = entity.CalcularNotaFinal(dto.Criterios);
        }
        
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

    /// <summary>Retorna dados para preenchimento automático do barema</summary>
    public async Task<BaremaDadosDto?> GetDadosBaremaAsync(long baremaId)
    {
        var barema = await _repository.GetByIdAsync(baremaId);
        if (barema == null) return null;

        var candidato = await _candidatoRepository.GetByIdAsync(barema.CandidatoId);
        var documentos = await _documentoRepository.GetByCandidatoIdAsync(barema.CandidatoId);

        return new BaremaDadosDto
        {
            BaremaId = barema.Id,
            NomeOrientador = candidato?.Orientador,
            NomeEstudante = candidato?.Nome,
            CursoGraduacao = candidato?.AreaPesquisa,
            NomeAvaliador = barema.Avaliador?.Nome,
            TipoBarema = barema.TipoBarema,
            Status = barema.Status,
            CriteriosJson = barema.CriteriosJson,
            NotaFinal = barema.NotaFinal,
            Observacoes = barema.Observacoes,
            Documentos = documentos.Select(d => _mapper.Map<DocumentoDto>(d))
        };
    }

    /// <summary>Finaliza automaticamente por eliminação na análise documental</summary>
    public async Task<BaremaDto> FinalizarPorEliminacaoAsync(long baremaId)
    {
        var entity = await _repository.GetByIdAsync(baremaId) ?? throw new Exception("Barema não encontrado");
        
        entity.NotaFinal = 0;
        entity.DataPreenchimento = DateTime.UtcNow;
        entity.Status = StatusBarema.Concluido;
        entity.Observacoes = "Candidato eliminado na Análise Documental";
        
        var dados = new Dictionary<string, object>
        {
            { "analiseDocumental", "eliminado" }
        };
        entity.CriteriosJson = JsonSerializer.Serialize(dados);
        
        var updated = await _repository.UpdateAsync(entity);
        return MapToDto(updated);
    }

    /// <summary>Retorna progresso de avaliação por candidato em um processo</summary>
    public async Task<IEnumerable<ProgressoCandidatoDto>> GetProgressoAsync(long processoId)
    {
        var candidatos = await _candidatoRepository.GetByProcessoIdAsync(processoId);
        var resultado = new List<ProgressoCandidatoDto>();

        foreach (var candidato in candidatos)
        {
            var baremas = await _repository.GetByCandidatoIdAsync(candidato.Id);
            var baremasAtivas = baremas.Where(b => b.Status != StatusBarema.Cancelado).ToList();

            var notasConcluidas = baremasAtivas
                .Where(b => b.Status == StatusBarema.Concluido)
                .Select(b => b.NotaFinal)
                .ToList();

            resultado.Add(new ProgressoCandidatoDto
            {
                CandidatoId = candidato.Id,
                CandidatoNome = candidato.Nome,
                NumeroInscricao = candidato.NumeroInscricao,
                AvaliadoresAtribuidos = baremasAtivas.Count,
                AvaliadoresConcluidos = notasConcluidas.Count,
                AvaliadoresNecessarios = baremasAtivas.Count,
                NotaFinal = notasConcluidas.Any() ? notasConcluidas.Average() : 0,
                Baremas = baremasAtivas.Select(b => new BaremaProgressoDto
                {
                    BaremaId = b.Id,
                    AvaliadorId = b.AvaliadorId,
                    AvaliadorNome = b.Avaliador?.Nome,
                    NotaFinal = b.NotaFinal,
                    Status = b.Status,
                    DataPreenchimento = b.DataPreenchimento
                }).ToList()
            });
        }

        return resultado;
    }

    /// <summary>Retorna progresso de avaliação de todos os processos</summary>
    public async Task<IEnumerable<ProgressoCandidatoDto>> GetProgressoAsync()
    {
        var todosBaremas = await _repository.GetAllAsync();
        var candidatosComBaremas = todosBaremas
            .Where(b => b.Status != StatusBarema.Cancelado)
            .GroupBy(b => b.CandidatoId)
            .ToList();

        var resultado = new List<ProgressoCandidatoDto>();

        foreach (var grupo in candidatosComBaremas)
        {
            var candidato = await _candidatoRepository.GetByIdAsync(grupo.Key);
            if (candidato == null) continue;

            var baremasAtivas = grupo.ToList();
            var notasConcluidas = baremasAtivas
                .Where(b => b.Status == StatusBarema.Concluido)
                .Select(b => b.NotaFinal)
                .ToList();

            resultado.Add(new ProgressoCandidatoDto
            {
                CandidatoId = candidato.Id,
                CandidatoNome = candidato.Nome,
                NumeroInscricao = candidato.NumeroInscricao,
                AvaliadoresAtribuidos = baremasAtivas.Count,
                AvaliadoresConcluidos = notasConcluidas.Count,
                AvaliadoresNecessarios = baremasAtivas.Count,
                NotaFinal = notasConcluidas.Any() ? notasConcluidas.Average() : 0,
                Baremas = baremasAtivas.Select(b => new BaremaProgressoDto
                {
                    BaremaId = b.Id,
                    AvaliadorId = b.AvaliadorId,
                    AvaliadorNome = b.Avaliador?.Nome,
                    NotaFinal = b.NotaFinal,
                    Status = b.Status,
                    DataPreenchimento = b.DataPreenchimento
                }).ToList()
            });
        }

        return resultado;
    }

    private BaremaDto MapToDto(Barema barema)
    {
        var dto = _mapper.Map<BaremaDto>(barema);
        dto.CandidatoNome = barema.Candidato?.Nome;
        dto.AvaliadorNome = barema.Avaliador?.Nome;
        dto.TipoBarema = barema.TipoBarema;
        
        if (!string.IsNullOrEmpty(barema.CriteriosJson))
        {
            try
            {
                dto.Criterios = JsonSerializer.Deserialize<Dictionary<string, float>>(barema.CriteriosJson);
            }
            catch (JsonException)
            {
                dto.Criterios = new Dictionary<string, float>();
            }
        }
        
        return dto;
    }
}
