using CestaBasica.Api.Data;
using CestaBasica.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CestaBasica.Api.Repositories;

public class ConfiguracoesRepository
{
    private readonly AppDbContext _context;

    public ConfiguracoesRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Configuracoes?> ObterAsync()
    {
        return await _context.ConfiguracoesSistema
            .FirstOrDefaultAsync();
    }

    public async Task AtualizarAsync(Configuracoes configuracao)
    {
        _context.ConfiguracoesSistema.Update(configuracao);
        await _context.SaveChangesAsync();
    }

    public async Task CriarAsync(Configuracoes configuracao)
    {
        _context.ConfiguracoesSistema.Add(configuracao);
        await _context.SaveChangesAsync();
    }
}