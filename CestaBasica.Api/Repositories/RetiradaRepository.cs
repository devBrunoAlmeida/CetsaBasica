using CestaBasica.Api.Data;
using CestaBasica.Api.Models;
using Microsoft.EntityFrameworkCore;
using CestaBasica.Shared.DTOs;

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

    public async Task<Retirada?> BuscarPorFuncionarioECestaAsync(
    int funcionarioId,
    int cestaId)
    {
        return await _context.Retiradas
            .Where(r =>
                r.FuncionarioId == funcionarioId &&
                r.CestaId == cestaId)
            .OrderByDescending(r => r.DataRetirada)
            .FirstOrDefaultAsync();
    }
    public async Task<List<Retirada>> ListarRecentesAsync()
    {
        return await _context.Retiradas
            .Include(r => r.Funcionario)
            .OrderByDescending(r => r.DataRetirada)
            .Take(10)
            .ToListAsync();
    }

    public async Task<List<RetiradaRecenteDto>> ListarRecentesDetalhadoAsync()
{
    return await _context.Retiradas
        .Include(x => x.Funcionario)
        .OrderByDescending(x => x.DataRetirada)
        .Take(30)
        .Select(x => new RetiradaRecenteDto
        {
            NomeCompleto = x.Funcionario.NomeCompleto,
            Matricula = x.Funcionario.Matricula,
            Setor = x.Funcionario.Setor,
            Status = "Retirado",
            DataRetirada = x.DataRetirada
        })
        .ToListAsync();
}
}