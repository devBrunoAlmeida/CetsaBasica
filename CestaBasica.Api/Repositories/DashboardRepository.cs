using CestaBasica.Api.Data;
using CestaBasica.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CestaBasica.Api.Repositories;

public class DashboardRepository
{
    private readonly AppDbContext _context;

    public DashboardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> ObterDadosAsync()
    {
        return new DashboardDto
        {
            TotalFuncionarios = await _context.Funcionarios.CountAsync(),
            TotalCestas = await _context.Cestas.CountAsync(),
            TotalRetiradas = await _context.Retiradas.CountAsync(),
            TotalNotificacoes = await _context.Notificacoes.CountAsync(),

            TotalCestasDisponiveis = await _context.Cestas
                .SumAsync(c => c.QuantidadeDisponivel)
        };
    }
}