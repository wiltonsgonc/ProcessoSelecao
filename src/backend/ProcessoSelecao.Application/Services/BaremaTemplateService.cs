using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Application.Services;

/// <summary>
/// Interface do serviço de Templates de Barema
/// </summary>
public interface IBaremaTemplateService
{
    Task<IEnumerable<BaremaTemplateDto>> GetAllAsync();
    Task<IEnumerable<BaremaTemplateDto>> GetActiveAsync();
    Task<BaremaTemplateDto?> GetByIdAsync(long id);
    Task<BaremaTemplateDto> CreateAsync(CreateBaremaTemplateDto dto);
    Task<BaremaTemplateDto> UpdateAsync(long id, UpdateBaremaTemplateDto dto);
    Task<BaremaTemplateDto?> CloneAsync(long templateId, string novoNome);
    Task DeleteAsync(long id);
    Task<BaremaTemplateDto?> ToggleAtivoAsync(long id);
}

/// <summary>
/// Serviço de Templates de Barema
/// </summary>
public class BaremaTemplateService : IBaremaTemplateService
{
    private readonly IBaremaTemplateRepository _repository;

    public BaremaTemplateService(IBaremaTemplateRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<BaremaTemplateDto>> GetAllAsync()
    {
        var templates = await _repository.GetAllAsync();
        return templates.Select(MapToDto);
    }

    public async Task<IEnumerable<BaremaTemplateDto>> GetActiveAsync()
    {
        var templates = await _repository.GetAllActiveAsync();
        return templates.Select(MapToDto);
    }

    public async Task<BaremaTemplateDto?> GetByIdAsync(long id)
    {
        var template = await _repository.GetWithItemsAsync(id);
        return template == null ? null : MapToDto(template);
    }

    public async Task<BaremaTemplateDto> CreateAsync(CreateBaremaTemplateDto dto)
    {
        var entity = new BaremaTemplate
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            TipoBarema = dto.TipoBarema,
            PontoMaximo = dto.PontoMaximo,
            Ativo = true,
            CriadoPor = dto.CriadoPor
        };

        foreach (var itemDto in dto.Itens)
        {
            entity.Itens.Add(new BaremaTemplateItem
            {
                Secao = itemDto.Secao,
                SecaoOrdem = itemDto.SecaoOrdem,
                Nome = itemDto.Nome,
                Ordem = itemDto.Ordem,
                NotaMinima = itemDto.NotaMinima,
                NotaMaxima = itemDto.NotaMaxima,
                Passo = itemDto.Passo,
                Obrigatorio = itemDto.Obrigatorio
            });
        }

        var created = await _repository.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<BaremaTemplateDto> UpdateAsync(long id, UpdateBaremaTemplateDto dto)
    {
        var entity = await _repository.GetWithItemsAsync(id)
            ?? throw new Exception("Template não encontrado");

        if (dto.Nome != null) entity.Nome = dto.Nome;
        if (dto.Descricao != null) entity.Descricao = dto.Descricao;
        if (dto.Ativo.HasValue) entity.Ativo = dto.Ativo.Value;

        if (dto.Itens != null)
        {
            entity.Itens.Clear();
            foreach (var itemDto in dto.Itens)
            {
                entity.Itens.Add(new BaremaTemplateItem
                {
                    Secao = itemDto.Secao,
                    SecaoOrdem = itemDto.SecaoOrdem,
                    Nome = itemDto.Nome,
                    Ordem = itemDto.Ordem,
                    NotaMinima = itemDto.NotaMinima,
                    NotaMaxima = itemDto.NotaMaxima,
                    Passo = itemDto.Passo,
                    Obrigatorio = itemDto.Obrigatorio
                });
            }
        }

        var updated = await _repository.UpdateAsync(entity);
        return MapToDto(updated);
    }

    public async Task<BaremaTemplateDto?> CloneAsync(long templateId, string novoNome)
    {
        var clone = await _repository.CloneAsync(templateId, novoNome);
        return clone == null ? null : MapToDto(clone);
    }

    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<BaremaTemplateDto?> ToggleAtivoAsync(long id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return null;

        entity.Ativo = !entity.Ativo;
        var updated = await _repository.UpdateAsync(entity);
        return MapToDto(updated);
    }

    private static BaremaTemplateDto MapToDto(BaremaTemplate entity)
    {
        return new BaremaTemplateDto
        {
            Id = entity.Id,
            Nome = entity.Nome,
            Descricao = entity.Descricao,
            TipoBarema = entity.TipoBarema,
            PontoMaximo = entity.PontoMaximo,
            Ativo = entity.Ativo,
            CriadoPor = entity.CriadoPor,
            DataCriacao = entity.DataCriacao,
            TotalItens = entity.Itens?.Count ?? 0,
            Itens = entity.Itens?.Select(i => new BaremaTemplateItemDto
            {
                Id = i.Id,
                TemplateId = i.TemplateId,
                Secao = i.Secao,
                SecaoOrdem = i.SecaoOrdem,
                Nome = i.Nome,
                Ordem = i.Ordem,
                NotaMinima = i.NotaMinima,
                NotaMaxima = i.NotaMaxima,
                Passo = i.Passo,
                Obrigatorio = i.Obrigatorio
            }).ToList() ?? new List<BaremaTemplateItemDto>()
        };
    }
}
