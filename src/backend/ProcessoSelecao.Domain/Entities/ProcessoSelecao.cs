using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Domain.Entities;

public class ProcessoSelecao : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public int VagasDisponiveis { get; set; }
    public StatusProcesso Status { get; set; } = StatusProcesso.Rascunho;
    
    public virtual ICollection<Candidato> Candidatos { get; set; } = new List<Candidato>();
    public virtual ICollection<Avaliador> Avaliadores { get; set; } = new List<Avaliador>();

    public void IniciarProcesso()
    {
        if (Status == StatusProcesso.Rascunho)
        {
            Status = StatusProcesso.Aberto;
            DataInicio = DateTime.UtcNow;
        }
    }

    public void FinalizarProcesso()
    {
        if (Status == StatusProcesso.EmAndamento || Status == StatusProcesso.Aberto)
        {
            Status = StatusProcesso.Finalizado;
            DataFim = DateTime.UtcNow;
        }
    }
}
