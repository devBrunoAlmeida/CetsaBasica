using Microsoft.EntityFrameworkCore;
using System.Linq;
using CestaBasica.Api.Data;
using CestaBasica.Api.Models;

public class HistoricoImportacaoRepository
{
    private readonly AppDbContext _context;

    public HistoricoImportacaoRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task AdicionarAsync(HistoricoImportacao historico)
{
    _context.HistoricoImportacoes.Add(historico);
    await _context.SaveChangesAsync();
}
    public async Task<List<HistoricoImportacao>> ListarAsync()
    {
        return await _context.HistoricoImportacoes
            .OrderByDescending(x => x.DataImportacao)
            .ToListAsync();
    }
}