namespace ProcessoSelecao.Blazor.Models;

public enum TipoDocumento
{
    HistoricoEscolar = 1,
    ComprovanteMatricula = 2,
    CartaIntencao = 3,
    CurriculumLatte = 4,
    CartaRecomendacao = 5
}

public enum StatusValidacao
{
    Pendente = 0,
    Validado = 1,
    Rejeitado = 2,
    EmAnalise = 3
}

public enum TipoAvaliador
{
    Interno = 1,
    Externo = 2
}

public enum StatusBarema
{
    Pendente = 0,
    EmPreenchimento = 1,
    Concluido = 2,
    Cancelado = 3
}

public enum StatusProcesso
{
    Rascunho = 0,
    Aberto = 1,
    EmAndamento = 2,
    Finalizado = 3,
    Cancelado = 4
}

public enum NivelCnpq
{
    NaoSeAplica = 0,
    Pq2 = 1,
    Pq1D = 2,
    Pq1C = 3,
    Pq1B = 4,
    Pq1A = 5,
    Dt2 = 6,
    Dt1 = 7
}
