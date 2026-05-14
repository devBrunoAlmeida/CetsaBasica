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

    public async Task<Cesta> CriarAsync(Cesta cesta)
    {
        _context.Cestas.Add(cesta);
        await _context.SaveChangesAsync();
        return cesta;
    }

    public async Task<List<Cesta>> ListarAsync()
    {
        return await _context.Cestas.ToListAsync();
    }

    public async Task<Cesta?> BuscarPorIdAsync(int id)
    {
        return await _context.Cestas.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Cesta> AtualizarAsync(Cesta cesta)
    {
        _context.Cestas.Update(cesta);
        await _context.SaveChangesAsync();
        return cesta;
    }
}