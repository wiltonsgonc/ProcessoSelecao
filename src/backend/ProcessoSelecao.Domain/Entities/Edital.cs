using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Domain.Entities;

public class Edital : BaseEntity
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataPublicacao { get; set; }
    public DateTime DataInicioInscricao { get; set; }
    public DateTime DataFimInscricao { get; set; }
    public decimal? ValorInscricao { get; set; }
    public string? TextoEdital { get; set; }
    public StatusEdital Status { get; set; } = StatusEdital.Rascunho;
    
    public List<string> LocaisProva { get; set; } = new();
    public List<string> Campi { get; set; } = new();
    public List<string> FormasInscricao { get; set; } = new();
    
    public bool ExigeRgCpf { get; set; } = true;
    public bool ExigeAnexoI { get; set; } = true;
    public bool ExigeCurriculoLattes { get; set; } = true;
    public bool ExigeCurriculoLattesOrientador { get; set; } = false;
    public bool ExigeAnexoII { get; set; } = false;
    public bool ExigeComprovanteMatricula { get; set; } = false;
    public bool ExigeHistoricoGraduacao { get; set; } = false;
    
    public virtual ICollection<OpcaoCurso> OpcoesCurso { get; set; } = new List<OpcaoCurso>();
    public virtual ICollection<Inscricao> Inscricoes { get; set; } = new List<Inscricao>();

    public bool EstaAberto()
    {
        var now = DateTime.UtcNow;
        return Status == StatusEdital.Publicado && 
               now >= DataInicioInscricao && 
               now <= DataFimInscricao;
    }
}
