using CestaBasica.Api.Data;
using CestaBasica.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CestaBasica.Api.Repositories;

public class DashboardRepository
{
    private readonly AppDbContext _context;

    public DashboardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> ObterDadosAsync(DateTime? dataInicio, DateTime? dataFim)
    {
        var hoje = DateTime.Today;

        dataInicio ??= hoje.AddYears(-1);
        dataFim ??= hoje;

        var fim = dataFim.Value.Date.AddDays(1);

        var retiradasQuery = _context.Retiradas
            .Include(r => r.Funcionario)
            .Where(r => r.DataRetirada >= dataInicio.Value && r.DataRetirada < fim);

        var notificacoesQuery = _context.Notificacoes
            .Where(n => n.DataEnvio >= dataInicio.Value && n.DataEnvio < fim);

        var totalFuncionarios = await _context.Funcionarios.CountAsync();

        var setores = await _context.Funcionarios
            .GroupBy(f => f.Setor)
            .Select(g => new
            {
                Setor = g.Key,
                Total = g.Count()
            })
            .ToListAsync();

        var distribuicaoPorSetor = setores
            .Select(s => new SetorDashboardDto
            {
                Setor = s.Setor,
                Porcentagem = totalFuncionarios == 0
                    ? 0
                    : Math.Round((decimal)s.Total * 100 / totalFuncionarios, 2)
            })
            .ToList();

        var evolucaoRetiradas = await retiradasQuery
            .GroupBy(r => new
            {
                r.DataRetirada.Year,
                r.DataRetirada.Month
            })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Month)
            .Select(g => new EvolucaoRetiradaDto
            {
                Mes = $"{g.Key.Month:00}/{g.Key.Year}",
                Total = g.Count()
            })
            .Take(6)
            .ToListAsync();

        var ultimasRetiradas = await retiradasQuery
            .OrderByDescending(r => r.DataRetirada)
            .Take(5)
            .Select(r => new UltimaRetiradaDto
            {
                Funcionario = r.Funcionario.Nome,
                Setor = r.Funcionario.Setor,
                Data = r.DataRetirada,
                Status = "Retirada"
            })
            .ToListAsync();

        var notificacoesRecentes = await notificacoesQuery
            .OrderByDescending(n => n.DataEnvio)
            .Take(5)
            .Select(n => new NotificacaoRecenteDto
            {
                Titulo = n.Titulo,
                Mensagem = n.Mensagem,
                DataEnvio = n.DataEnvio
            })
            .ToListAsync();

        return new DashboardDto
        {
            TotalFuncionarios = totalFuncionarios,
            TotalCestas = await _context.Cestas.CountAsync(),
            TotalRetiradas = await retiradasQuery.CountAsync(),
            TotalNotificacoes = await notificacoesQuery.CountAsync(),

            TotalCestasDisponiveis = await _context.Cestas
                .Select(c => (int?)c.QuantidadeDisponivel)
                .SumAsync() ?? 0,

            DistribuicaoPorSetor = distribuicaoPorSetor,
            EvolucaoRetiradas = evolucaoRetiradas,
            UltimasRetiradas = ultimasRetiradas,
            NotificacoesRecentes = notificacoesRecentes
        };
    }
}