using CestaBasica.Api.Data;
using CestaBasica.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CestaBasica.Api.Repositories;

public class NotificacaoRepository
{
    private readonly AppDbContext _context;

    public NotificacaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Notificacao> CriarAsync(Notificacao notificacao)
    {
        _context.Notificacoes.Add(notificacao);
        await _context.SaveChangesAsync();
        return notificacao;
    }

    public async Task<List<Notificacao>> ListarAsync()
    {
        return await _context.Notificacoes
            .Include(n => n.Funcionario)
            .OrderByDescending(n => n.DataEnvio)
            .ToListAsync();
    }

    public async Task<List<Notificacao>> ListarPorFuncionarioAsync(int funcionarioId)
    {
        return await _context.Notificacoes
            .Where(n => n.FuncionarioId == funcionarioId)
            .OrderByDescending(n => n.DataEnvio)
            .ToListAsync();
    }
}