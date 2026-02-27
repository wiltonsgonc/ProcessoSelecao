using AutoMapper;
using ProcessoSelecao.Application.DTOs;
using DomainEntities = ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<DomainEntities.Candidato, CandidatoDto>();
        CreateMap<CreateCandidatoDto, DomainEntities.Candidato>();
        CreateMap<UpdateCandidatoDto, DomainEntities.Candidato>();
        
        CreateMap<DomainEntities.Documento, DocumentoDto>();
        CreateMap<CreateDocumentoDto, DomainEntities.Documento>();
        
        CreateMap<DomainEntities.Avaliador, AvaliadorDto>();
        CreateMap<CreateAvaliadorDto, DomainEntities.Avaliador>();
        CreateMap<UpdateAvaliadorDto, DomainEntities.Avaliador>();
        
        CreateMap<DomainEntities.Barema, BaremaDto>();
        CreateMap<DomainEntities.ProcessoSelecao, ProcessoSelecaoDto>();
        CreateMap<CreateProcessoSelecaoDto, DomainEntities.ProcessoSelecao>();
        CreateMap<UpdateProcessoSelecaoDto, DomainEntities.ProcessoSelecao>();

        CreateMap<DomainEntities.Edital, EditalDto>();
        CreateMap<EditalCreateDto, DomainEntities.Edital>();
        CreateMap<EditalUpdateDto, DomainEntities.Edital>();

        CreateMap<DomainEntities.OpcaoCurso, OpcaoCursoDto>();
        CreateMap<OpcaoCursoCreateDto, DomainEntities.OpcaoCurso>();

        CreateMap<DomainEntities.Inscricao, InscricaoDto>();
        CreateMap<InscricaoCreateDto, DomainEntities.Inscricao>();

        CreateMap<DomainEntities.DocumentoInscricao, DocumentoInscricaoDto>();
    }
}
