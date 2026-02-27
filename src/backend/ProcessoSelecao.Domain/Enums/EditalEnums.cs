namespace ProcessoSelecao.Domain.Enums;

/// <summary>
/// Status do edital no sistema
/// </summary>
public enum StatusEdital
{
    Rascunho,
    Publicado,
    Encerrado,
    Cancelado
}

/// <summary>
/// Status da inscrição do candidato
/// </summary>
public enum StatusInscricao
{
    Pendente,
    Completa,
    Confirmada,
    Cancelada
}

/// <summary>
/// Tipos de documentos exigidos na inscrição
/// </summary>
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

/// <summary>
/// Forma de realização da inscrição
/// </summary>
public enum FormaInscricao
{
    Online,
    Presencial
}

/// <summary>
/// Tipos de deficiência para questões de accessibility
/// </summary>
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
