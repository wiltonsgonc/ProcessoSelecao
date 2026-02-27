using AutoMapper;
using ProcessoSelecao.Application.DTOs;
using DomainEntities = ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Application;

/// <summary>
/// Configuração de mapeamentos entre entidades e DTOs usando AutoMapper
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Candidato
        CreateMap<DomainEntities.Candidato, CandidatoDto>();
        CreateMap<CreateCandidatoDto, DomainEntities.Candidato>();
        CreateMap<UpdateCandidatoDto, DomainEntities.Candidato>();
        
        // Documento
        CreateMap<DomainEntities.Documento, DocumentoDto>();
        CreateMap<CreateDocumentoDto, DomainEntities.Documento>();
        
        // Avaliador
        CreateMap<DomainEntities.Avaliador, AvaliadorDto>();
        CreateMap<CreateAvaliadorDto, DomainEntities.Avaliador>();
        CreateMap<UpdateAvaliadorDto, DomainEntities.Avaliador>();
        
        // Barema
        CreateMap<DomainEntities.Barema, BaremaDto>();
        
        // ProcessoSelecao
        CreateMap<DomainEntities.ProcessoSelecao, ProcessoSelecaoDto>();
        CreateMap<CreateProcessoSelecaoDto, DomainEntities.ProcessoSelecao>();
        CreateMap<UpdateProcessoSelecaoDto, DomainEntities.ProcessoSelecao>();

        // Edital
        CreateMap<DomainEntities.Edital, EditalDto>();
        CreateMap<EditalCreateDto, DomainEntities.Edital>();
        CreateMap<EditalUpdateDto, DomainEntities.Edital>();

        // OpcaoCurso
        CreateMap<DomainEntities.OpcaoCurso, OpcaoCursoDto>();
        CreateMap<OpcaoCursoCreateDto, DomainEntities.OpcaoCurso>();

        // Inscricao
        CreateMap<DomainEntities.Inscricao, InscricaoDto>();
        CreateMap<InscricaoCreateDto, DomainEntities.Inscricao>();

        // DocumentoInscricao
        CreateMap<DomainEntities.DocumentoInscricao, DocumentoInscricaoDto>();
    }
}
