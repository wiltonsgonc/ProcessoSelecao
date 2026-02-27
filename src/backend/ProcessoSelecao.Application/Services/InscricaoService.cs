using AutoMapper;
using ProcessoSelecao.Application.DTOs;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Application.Services;

public class InscricaoService
{
    private readonly IInscricaoRepository _inscricaoRepository;
    private readonly IEditalRepository _editalRepository;
    private readonly IDocumentoInscricaoRepository _documentoInscricaoRepository;
    private readonly IMapper _mapper;

    public InscricaoService(
        IInscricaoRepository inscricaoRepository,
        IEditalRepository editalRepository,
        IDocumentoInscricaoRepository documentoInscricaoRepository,
        IMapper mapper)
    {
        _inscricaoRepository = inscricaoRepository;
        _editalRepository = editalRepository;
        _documentoInscricaoRepository = documentoInscricaoRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<InscricaoDto>> GetAllAsync()
    {
        var inscricoes = await _inscricaoRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<InscricaoDto>>(inscricoes);
    }

    public async Task<IEnumerable<InscricaoDto>> GetByEditalIdAsync(int editalId)
    {
        var inscricoes = await _inscricaoRepository.GetByEditalIdAsync(editalId);
        return _mapper.Map<IEnumerable<InscricaoDto>>(inscricoes);
    }

    public async Task<InscricaoDto?> GetByIdAsync(int id)
    {
        var inscricao = await _inscricaoRepository.GetByIdWithDocumentsAsync(id);
        if (inscricao == null) return null;
        
        var dto = _mapper.Map<InscricaoDto>(inscricao);
        dto.NomeEdital = inscricao.Edital?.Titulo;
        dto.NomeOpcaoCurso = inscricao.OpcaoCurso?.Nome;
        return dto;
    }

    public async Task<InscricaoDto> CreateAsync(InscricaoCreateDto createDto)
    {
        var edital = await _editalRepository.GetByIdAsync(createDto.EditalId);
        if (edital == null)
            throw new InvalidOperationException("Edital não encontrado");

        if (!edital.EstaAberto())
            throw new InvalidOperationException("Período de inscrição encerrado");

        var existing = await _inscricaoRepository.GetByCpfAsync(createDto.NumeroDocumento, createDto.EditalId);
        if (existing != null)
            throw new InvalidOperationException("CPF já inscrito neste edital");

        var inscricao = new Inscricao
        {
            EditalId = createDto.EditalId,
            OpcaoCursoId = createDto.OpcaoCursoId,
            Nome = createDto.Nome,
            DataNascimento = createDto.DataNascimento,
            TipoDocumento = createDto.TipoDocumento,
            NumeroDocumento = createDto.NumeroDocumento,
            Email = createDto.Email,
            Telefone1 = createDto.Telefone1,
            Telefone2 = createDto.Telefone2,
            AceitaPoliticaPrivacidade = createDto.AceitaPoliticaPrivacidade,
            PaisNatal = createDto.PaisNatal,
            EstadoNatal = createDto.EstadoNatal,
            Naturalidade = createDto.Naturalidade,
            NomeSocial = createDto.NomeSocial,
            EstadoCivil = createDto.EstadoCivil,
            Nacionalidade = createDto.Nacionalidade,
            Sexo = createDto.Sexo,
            CorRaca = createDto.CorRaca,
            AutorizaDadosPessoais = createDto.AutorizaDadosPessoais,
            TipoVisto = createDto.TipoVisto,
            NumeroRegistroGeral = createDto.NumeroRegistroGeral,
            DataVencimentoRg = createDto.DataVencimentoRg,
            FormaInscricao = createDto.FormaInscricao,
            LocalRealizacaoProva = createDto.LocalRealizacaoProva,
            CampusRealizacaoProva = createDto.CampusRealizacaoProva,
            DefFisica = createDto.DefFisica,
            DefAuditiva = createDto.DefAuditiva,
            DefFala = createDto.DefFala,
            DefVisual = createDto.DefVisual,
            DefMental = createDto.DefMental,
            DefIntelectual = createDto.DefIntelectual,
            DefReabilitado = createDto.DefReabilitado,
            DefMultipla = createDto.DefMultipla,
            DefOutrasNecessidades = createDto.DefOutrasNecessidades,
            DataInscricao = DateTime.UtcNow,
            Status = StatusInscricao.Pendente
        };

        var created = await _inscricaoRepository.AddAsync(inscricao);
        
        var dto = _mapper.Map<InscricaoDto>(created);
        dto.NomeEdital = edital.Titulo;
        return dto;
    }

    public async Task<InscricaoDto?> UpdateAsync(int id, InscricaoCreateDto updateDto)
    {
        var inscricao = await _inscricaoRepository.GetByIdAsync(id);
        if (inscricao == null) return null;

        if (inscricao.Status != StatusInscricao.Pendente)
            throw new InvalidOperationException("Inscrição já confirmada não pode ser alterada");

        _mapper.Map(updateDto, inscricao);
        
        var updated = await _inscricaoRepository.UpdateAsync(inscricao);
        return _mapper.Map<InscricaoDto>(updated);
    }

    public async Task<bool> ConfirmarAsync(int id)
    {
        var inscricao = await _inscricaoRepository.GetByIdAsync(id);
        if (inscricao == null) return false;

        if (!inscricao.AceitaPoliticaPrivacidade)
            throw new InvalidOperationException("Candidato deve aceitar a política de privacidade");

        inscricao.Status = StatusInscricao.Confirmada;
        await _inscricaoRepository.UpdateAsync(inscricao);
        return true;
    }

    public async Task<bool> CancelarAsync(int id)
    {
        var inscricao = await _inscricaoRepository.GetByIdAsync(id);
        if (inscricao == null) return false;

        inscricao.Status = StatusInscricao.Cancelada;
        await _inscricaoRepository.UpdateAsync(inscricao);
        return true;
    }

    public async Task<bool> ValidarDocumentosAsync(int id)
    {
        var inscricao = await _inscricaoRepository.GetByIdWithDocumentsAsync(id);
        if (inscricao == null) return false;

        var documentosObrigatorios = new[]
        {
            TipoDocumentoInscricao.RgCpf,
            TipoDocumentoInscricao.AnexoI,
            TipoDocumentoInscricao.ComprovanteMatricula
        };

        var docsEnviados = inscricao.Documentos.Select(d => d.Tipo).ToHashSet();
        var faltan = documentosObrigatorios.Where(t => !docsEnviados.Contains(t)).ToList();

        if (faltan.Any())
            throw new InvalidOperationException($"Documentos obrigatórios faltando: {string.Join(", ", faltan)}");

        return true;
    }
}
