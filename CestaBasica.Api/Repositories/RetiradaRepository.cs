using CestaBasica.Api.Data;
using CestaBasica.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CestaBasica.Api.Repositories;

public class RetiradaRepository
{
    private readonly AppDbContext _context;

    public RetiradaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Retirada> CriarAsync(Retirada retirada)
    {
        _context.Retiradas.Add(retirada);
        await _context.SaveChangesAsync();
        return retirada;
    }

    public async Task<bool> JaRetirouAsync(int funcionarioId, int cestaId)
    {
        return await _context.Retiradas
            .AnyAsync(r => r.FuncionarioId == funcionarioId && r.CestaId == cestaId);
    }

    public async Task<List<Retirada>> ListarAsync()
    {
        return await _context.Retiradas
            .Include(r => r.Funcionario)
            .Include(r => r.Cesta)
            .ToListAsync();
    }
}