using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Domain.Entities;

/// <summary>
/// Entidade que representa um Edital de Processo Seletivo
/// </summary>
public class Edital : BaseEntity
{
    /// <summary>Título do edital</summary>
    public string Titulo { get; set; } = string.Empty;
    
    /// <summary>Descrição resumida do edital</summary>
    public string? Descricao { get; set; }
    
    /// <summary>Data de publicação no diário oficial</summary>
    public DateTime DataPublicacao { get; set; }
    
    /// <summary>Data de início das inscrições</summary>
    public DateTime DataInicioInscricao { get; set; }
    
    /// <summary>Data de término das inscrições</summary>
    public DateTime DataFimInscricao { get; set; }
    
    /// <summary>Valor da taxa de inscrição</summary>
    public decimal? ValorInscricao { get; set; }
    
    /// <summary>Texto completo do edital em formato HTML/Rich Text</summary>
    public string? TextoEdital { get; set; }
    
    /// <summary>Status atual do edital</summary>
    public StatusEdital Status { get; set; } = StatusEdital.Rascunho;
    
    /// <summary>Locais onde será aplicada a prova</summary>
    public List<string> LocaisProva { get; set; } = new();
    
    /// <summary>Campi oferecidos no processo</summary>
    public List<string> Campi { get; set; } = new();
    
    /// <summary>Formas de inscrição disponíveis</summary>
    public List<string> FormasInscricao { get; set; } = new();
    
    /// <summary>Exige documento de identidade com CPF</summary>
    public bool ExigeRgCpf { get; set; } = true;
    
    /// <summary>Exige preenchimento do Anexo I</summary>
    public bool ExigeAnexoI { get; set; } = true;
    
    /// <summary>Exige currículo Lattes do candidato</summary>
    public bool ExigeCurriculoLattes { get; set; } = true;
    
    /// <summary>Exige currículo Lattes do orientador</summary>
    public bool ExigeCurriculoLattesOrientador { get; set; } = false;
    
    /// <summary>Exige preenchimento do Anexo II</summary>
    public bool ExigeAnexoII { get; set; } = false;
    
    /// <summary>Exige comprovante de matrícula</summary>
    public bool ExigeComprovanteMatricula { get; set; } = false;
    
    /// <summary>Exige histórico de graduação</summary>
    public bool ExigeHistoricoGraduacao { get; set; } = false;
    
    /// <summary>Cursos/opções disponíveis neste edital</summary>
    public virtual ICollection<OpcaoCurso> OpcoesCurso { get; set; } = new List<OpcaoCurso>();
    
    /// <summary>Inscrições realizadas neste edital</summary>
    public virtual ICollection<Inscricao> Inscricoes { get; set; } = new List<Inscricao>();

    /// <summary>
    /// Verifica se o edital está aberto para inscrições
    /// </summary>
    /// <returns>True se estiver publicado e dentro do período de inscrição</returns>
    public bool EstaAberto()
    {
        var now = DateTime.UtcNow;
        return Status == StatusEdital.Publicado && 
               now >= DataInicioInscricao && 
               now <= DataFimInscricao;
    }
}
