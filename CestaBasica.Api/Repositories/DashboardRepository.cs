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
        var hoje = DateTime.UtcNow.Date;

        if (dataFim.HasValue && dataFim.Value.Date > hoje)
            throw new ArgumentException("A data final não pode ser maior que a data atual.");

        if (dataInicio.HasValue && dataInicio.Value.Date > hoje)
            throw new ArgumentException("A data inicial não pode ser maior que a data atual.");

        if (dataInicio.HasValue && dataFim.HasValue && dataInicio.Value.Date > dataFim.Value.Date)
            throw new ArgumentException("A data inicial não pode ser maior que a data final.");

        dataInicio = dataInicio.HasValue
            ? DateTime.SpecifyKind(dataInicio.Value.Date, DateTimeKind.Utc)
            : hoje.AddYears(-1).Date;

        dataFim = dataFim.HasValue
            ? DateTime.SpecifyKind(dataFim.Value.Date, DateTimeKind.Utc)
            : hoje.Date;

        var fim = dataFim.Value.AddDays(1);

        var retiradasQuery = _context.Retiradas
            .Include(r => r.Funcionario)
            .Where(r => r.DataRetirada >= dataInicio.Value &&
                        r.DataRetirada < fim);

        var notificacoesQuery = _context.Notificacoes
            .Where(n => n.DataEnvio >= dataInicio.Value &&
                        n.DataEnvio < fim);

        var totalFuncionarios = await _context.Funcionarios.CountAsync();

        var totalRetiradasPeriodo = await retiradasQuery.CountAsync();

        var setores = await retiradasQuery
            .GroupBy(r => r.Funcionario.Setor)
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
                Porcentagem = totalRetiradasPeriodo == 0
                    ? 0
                    : Math.Round((decimal)s.Total * 100 / totalRetiradasPeriodo, 2)
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
                Funcionario = r.Funcionario.NomeCompleto,
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
        var totalCestasEstoque = await _context.Cestas
    .Where(c => c.Ativa)
    .Select(c => (int?)c.QuantidadeDisponivel)
    .SumAsync() ?? 0;

        var totalCestasDisponiveis = Math.Max(0, totalCestasEstoque);


        return new DashboardDto
        {
            TotalFuncionarios = totalFuncionarios,

            TotalCestas = await _context.Cestas
        .Where(c => c.Ativa)
        .CountAsync(),

            TotalRetiradas = totalRetiradasPeriodo,
            TotalNotificacoes = await notificacoesQuery.CountAsync(),

            TotalCestasDisponiveis = totalCestasDisponiveis,

            DistribuicaoPorSetor = distribuicaoPorSetor,
            EvolucaoRetiradas = evolucaoRetiradas,
            UltimasRetiradas = ultimasRetiradas,
            NotificacoesRecentes = notificacoesRecentes
        };
    }
}