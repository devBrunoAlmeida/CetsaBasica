using CestaBasica.Api.Data;
using CestaBasica.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CestaBasica.Api.Repositories;

public class CestaRepository
{
    private readonly AppDbContext _context;

    public CestaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Cesta>> ListarAsync()
    {
        return await _context.Cestas
            .OrderByDescending(c => c.DataCriacao)
            .ToListAsync();
    }

    public async Task<Cesta?> ObterPorIdAsync(int id)
    {
        return await _context.Cestas.FindAsync(id);
    }

    public async Task AtualizarAsync(Cesta cesta)
    {
        _context.Cestas.Update(cesta);
        await _context.SaveChangesAsync();
    }
    public async Task<List<Cesta>> ListarAtivasAsync()
    {
        return await _context.Cestas
            .Where(c => c.Ativa)
            .OrderByDescending(c => c.DataCriacao)
            .ToListAsync();
    }
    public async Task<Cesta?> ObterCestaAtivaAsync()
    {
        return await _context.Cestas
            .Where(c => c.Ativa && c.QuantidadeDisponivel > 0)
            .OrderByDescending(c => c.Id)
            .FirstOrDefaultAsync();
    }
    public async Task AdicionarAsync(Cesta cesta)
    {
        await _context.Cestas.AddAsync(cesta);
        await _context.SaveChangesAsync();
    }
}