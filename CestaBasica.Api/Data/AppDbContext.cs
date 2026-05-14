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
}