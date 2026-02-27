using AutoMapper;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Application.Services;

public class EditalService
{
    private readonly IEditalRepository _editalRepository;
    private readonly IOpcaoCursoRepository _opcaoCursoRepository;
    private readonly IMapper _mapper;

    public EditalService(IEditalRepository editalRepository, IOpcaoCursoRepository opcaoCursoRepository, IMapper mapper)
    {
        _editalRepository = editalRepository;
        _opcaoCursoRepository = opcaoCursoRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EditalDto>> GetAllAsync()
    {
        var editais = await _editalRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<EditalDto>>(editais);
    }

    public async Task<IEnumerable<EditalDto>> GetPublishedAsync()
    {
        var editais = await _editalRepository.GetPublishedAsync();
        return _mapper.Map<IEnumerable<EditalDto>>(editais);
    }

    public async Task<EditalDto?> GetByIdAsync(int id)
    {
        var edital = await _editalRepository.GetByIdWithOptionsAsync(id);
        if (edital == null) return null;
        
        var dto = _mapper.Map<EditalDto>(edital);
        dto.EstaAberto = edital.EstaAberto();
        return dto;
    }

    public async Task<EditalDto> CreateAsync(EditalCreateDto createDto)
    {
        var edital = new Edital
        {
            Titulo = createDto.Titulo,
            Descricao = createDto.Descricao,
            DataPublicacao = createDto.DataPublicacao,
            DataInicioInscricao = createDto.DataInicioInscricao,
            DataFimInscricao = createDto.DataFimInscricao,
            ValorInscricao = createDto.ValorInscricao,
            TextoEdital = createDto.TextoEdital,
            Status = StatusEdital.Rascunho,
            LocaisProva = createDto.LocaisProva,
            Campi = createDto.Campi,
            FormasInscricao = createDto.FormasInscricao,
            ExigeRgCpf = createDto.ExigeRgCpf,
            ExigeAnexoI = createDto.ExigeAnexoI,
            ExigeCurriculoLattes = createDto.ExigeCurriculoLattes,
            ExigeCurriculoLattesOrientador = createDto.ExigeCurriculoLattesOrientador,
            ExigeAnexoII = createDto.ExigeAnexoII,
            ExigeComprovanteMatricula = createDto.ExigeComprovanteMatricula,
            ExigeHistoricoGraduacao = createDto.ExigeHistoricoGraduacao
        };

        foreach (var opcao in createDto.OpcoesCurso)
        {
            edital.OpcoesCurso.Add(new OpcaoCurso
            {
                Nome = opcao.Nome,
                Descricao = opcao.Descricao,
                Vagas = opcao.Vagas,
                Campus = opcao.Campus,
                LocalProva = opcao.LocalProva
            });
        }

        var created = await _editalRepository.AddAsync(edital);
        return _mapper.Map<EditalDto>(created);
    }

    public async Task<EditalDto?> UpdateAsync(EditalUpdateDto updateDto)
    {
        var edital = await _editalRepository.GetByIdWithOptionsAsync(updateDto.Id);
        if (edital == null) return null;

        edital.Titulo = updateDto.Titulo;
        edital.Descricao = updateDto.Descricao;
        edital.DataPublicacao = updateDto.DataPublicacao;
        edital.DataInicioInscricao = updateDto.DataInicioInscricao;
        edital.DataFimInscricao = updateDto.DataFimInscricao;
        edital.ValorInscricao = updateDto.ValorInscricao;
        edital.TextoEdital = updateDto.TextoEdital;
        edital.Status = updateDto.Status;
        edital.LocaisProva = updateDto.LocaisProva;
        edital.Campi = updateDto.Campi;
        edital.FormasInscricao = updateDto.FormasInscricao;
        edital.ExigeRgCpf = updateDto.ExigeRgCpf;
        edital.ExigeAnexoI = updateDto.ExigeAnexoI;
        edital.ExigeCurriculoLattes = updateDto.ExigeCurriculoLattes;
        edital.ExigeCurriculoLattesOrientador = updateDto.ExigeCurriculoLattesOrientador;
        edital.ExigeAnexoII = updateDto.ExigeAnexoII;
        edital.ExigeComprovanteMatricula = updateDto.ExigeComprovanteMatricula;
        edital.ExigeHistoricoGraduacao = updateDto.ExigeHistoricoGraduacao;

        var updated = await _editalRepository.UpdateAsync(edital);
        return _mapper.Map<EditalDto>(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await _editalRepository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> PublicarAsync(int id)
    {
        var edital = await _editalRepository.GetByIdAsync(id);
        if (edital == null) return false;

        edital.Status = StatusEdital.Publicado;
        await _editalRepository.UpdateAsync(edital);
        return true;
    }

    public async Task<bool> EncerrarAsync(int id)
    {
        var edital = await _editalRepository.GetByIdAsync(id);
        if (edital == null) return false;

        edital.Status = StatusEdital.Encerrado;
        await _editalRepository.UpdateAsync(edital);
        return true;
    }
}
