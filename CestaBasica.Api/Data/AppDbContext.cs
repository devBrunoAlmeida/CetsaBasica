using Microsoft.EntityFrameworkCore;
using CestaBasica.Api.Models;

namespace CestaBasica.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }


    public DbSet<Funcionario> Funcionarios { get; set; }
    public DbSet<Cesta> Cestas { get; set; }
    public DbSet<Retirada> Retiradas { get; set; }
    public DbSet<Notificacao> Notificacoes { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<HistoricoImportacao> HistoricoImportacoes { get; set; }
    public DbSet<Configuracoes> ConfiguracoesSistema { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<HistoricoImportacao>().ToTable("historico_importacoes");
        modelBuilder.Entity<Funcionario>().ToTable("funcionarios");
        modelBuilder.Entity<Cesta>().ToTable("cestas");
        modelBuilder.Entity<Retirada>().ToTable("retiradas");
        modelBuilder.Entity<Notificacao>().ToTable("notificacoes");
        modelBuilder.Entity<Usuario>().ToTable("usuarios");
        modelBuilder.Entity<Configuracoes>().ToTable("configuracao_sistema");
    }
}