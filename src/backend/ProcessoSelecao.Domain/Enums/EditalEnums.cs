namespace ProcessoSelecao.Domain.Enums;

public enum StatusEdital
{
    Rascunho,
    Publicado,
    Encerrado,
    Cancelado
}

public enum StatusInscricao
{
    Pendente,
    Completa,
    Confirmada,
    Cancelada
}

public enum TipoDocumentoInscricao
{
    RgCpf,
    AnexoI,
    CurriculoLattesCandidato,
    CurriculoLattesOrientador,
    AnexoII,
    ComprovanteMatricula,
    HistoricoGraduacao
}

public enum FormaInscricao
{
    Online,
    Presencial
}

public enum TipoDeficiencia
{
    Nenhuma,
    Fisica,
    Auditiva,
    Fala,
    Visual,
    Mental,
    Intelectual,
    ReabilitadoBR,
    Multipla,
    Outras
}
