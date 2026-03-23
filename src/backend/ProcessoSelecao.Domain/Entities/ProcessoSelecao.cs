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
            if (DataInicio == default)
            {
                DataInicio = DateTime.UtcNow;
            }
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

    /// <summary>
    /// Verifica se o prazo do processo expirou e encerra automaticamente
    /// </summary>
    public bool VerificarPrazoExpirado()
    {
        if (DataFim.HasValue && Status != StatusProcesso.Finalizado)
        {
            if (DateTime.UtcNow > DataFim.Value)
            {
                Status = StatusProcesso.Finalizado;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Verifica se o processo está dentro do prazo de vigência (independente do status)
    /// </summary>
    public bool EstaDentroDoPrazo()
    {
        var now = DateTime.UtcNow;
        
        if (DataInicio > now)
            return false;
            
        if (DataFim.HasValue && now > DataFim.Value)
            return false;
            
        return true;
    }

    /// <summary>
    /// Reverte o status para EmAndamento se o processo estiver Finalizado mas o prazo ainda for válido
    /// </summary>
    public void ReverterSePrazoValido()
    {
        if (Status == StatusProcesso.Finalizado && EstaDentroDoPrazo())
        {
            Status = StatusProcesso.EmAndamento;
        }
    }
}
