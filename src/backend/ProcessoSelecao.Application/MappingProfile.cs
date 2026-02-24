using AutoMapper;
using ProcessoSelecao.Application.DTOs;
using DomainEntities = ProcessoSelecao.Domain.Entities;

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
    }
}
