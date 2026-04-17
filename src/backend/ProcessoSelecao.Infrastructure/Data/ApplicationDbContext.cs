using Microsoft.EntityFrameworkCore;
using DomainEntities = ProcessoSelecao.Domain.Entities;

namespace ProcessoSelecao.Infrastructure.Data;

/// <summary>
/// Contexto do Entity Framework Core para o banco de dados do ProcessoSelecao
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    /// <summary>Candidatos cadastrados no sistema</summary>
    public DbSet<DomainEntities.Candidato> Candidatos => Set<DomainEntities.Candidato>();
    
    /// <summary>Documentos dos candidatos</summary>
    public DbSet<DomainEntities.Documento> Documentos => Set<DomainEntities.Documento>();
    
    /// <summary>Avaliadores dos processos</summary>
    public DbSet<DomainEntities.Avaliador> Avaliadores => Set<DomainEntities.Avaliador>();
    
    /// <summary>Baremas/Avaliações</summary>
    public DbSet<DomainEntities.Barema> Baremas => Set<DomainEntities.Barema>();
    
    /// <summary>Processos de Seleção</summary>
    public DbSet<DomainEntities.ProcessoSelecao> ProcessosSelecao => Set<DomainEntities.ProcessoSelecao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ============================================
        // Candidato
        // ============================================
        modelBuilder.Entity<DomainEntities.Candidato>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Cpf).IsRequired().HasMaxLength(50);
            entity.Property(e => e.RG).HasMaxLength(50);
            entity.Property(e => e.Telefone).HasMaxLength(30);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.AreaPesquisa).HasMaxLength(200);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Cpf).IsUnique();
        });

        // ============================================
        // Documento
        // ============================================
        modelBuilder.Entity<DomainEntities.Documento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NomeArquivo).IsRequired().HasMaxLength(300);
            entity.Property(e => e.CaminhoLocal).IsRequired().HasMaxLength(500);
            entity.Property(e => e.LinkUrl).HasMaxLength(500);
            entity.Property(e => e.HashValidacao).HasMaxLength(100);
            entity.Property(e => e.MotivoRejeicao).HasMaxLength(500);
            entity.HasOne(e => e.Candidato).WithMany(c => c.Documentos).HasForeignKey(e => e.CandidatoId);
        });

        // ============================================
        // Avaliador
        // ============================================
        modelBuilder.Entity<DomainEntities.Avaliador>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.AreaEspecializacao).HasMaxLength(200);
            entity.Property(e => e.Instituicao).HasMaxLength(200);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // ============================================
        // Barema
        // ============================================
        modelBuilder.Entity<DomainEntities.Barema>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CriteriosJson).HasColumnType("nvarchar(max)");
            entity.Property(e => e.Observacoes).HasMaxLength(1000);
            entity.HasOne(e => e.Candidato).WithMany(c => c.Baremas).HasForeignKey(e => e.CandidatoId);
            entity.HasOne(e => e.Avaliador).WithMany(a => a.Baremas).HasForeignKey(e => e.AvaliadorId);
        });

        // ============================================
        // ProcessoSelecao
        // ============================================
        modelBuilder.Entity<DomainEntities.ProcessoSelecao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd().UseIdentityColumn(1, 1);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descricao).HasMaxLength(1000);
        });
    }
}
