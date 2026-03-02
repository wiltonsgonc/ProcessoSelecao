using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Domain.Entities;

/// <summary>
/// Entidade que representa um processo de seleção
/// </summary>
public class ProcessoSelecao : BaseEntity
{
    /// <summary>Nome do processo de seleção</summary>
    public string Nome { get; set; } = string.Empty;
    
    /// <summary>Descrição do processo</summary>
    public string? Descricao { get; set; }
    
    /// <summary>Data de início</summary>
    public DateTime DataInicio { get; set; }
    
    /// <summary>Data de término</summary>
    public DateTime? DataFim { get; set; }
    
    /// <summary>Número de vagas disponíveis</summary>
    public int VagasDisponiveis { get; set; }
    
    /// <summary>Status atual do processo</summary>
    public StatusProcesso Status { get; set; } = StatusProcesso.Rascunho;
    
    /// <summary>Candidatos inscritos neste processo</summary>
    public virtual ICollection<Candidato> Candidatos { get; set; } = new List<Candidato>();
    
    /// <summary>Avaliadores designados para este processo</summary>
    public virtual ICollection<Avaliador> Avaliadores { get; set; } = new List<Avaliador>();

    /// <summary>
    /// Inicia o processo de seleção (altera status para Aberto)
    /// </summary>
    public void IniciarProcesso()
    {
        if (Status == StatusProcesso.Rascunho)
        {
            Status = StatusProcesso.Aberto;
            DataInicio = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Finaliza o processo de seleção (altera status para Finalizado)
    /// </summary>
    public void FinalizarProcesso()
    {
        if (Status == StatusProcesso.EmAndamento || Status == StatusProcesso.Aberto)
        {
            Status = StatusProcesso.Finalizado;
            DataFim = DateTime.UtcNow;
        }
    }
}
