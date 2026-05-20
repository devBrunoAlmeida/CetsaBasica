using CestaBasica.Shared.DTOs;
using CestaBasica.Api.Repositories;

namespace CestaBasica.Api.Services;

public class DashboardService
{
    private readonly DashboardRepository _repo;

    public DashboardService(DashboardRepository repo)
    {
        _repo = repo;
    }

    public async Task<DashboardDto> ObterDadosAsync(DateTime? dataInicio, DateTime? dataFim)
    {
        return await _repo.ObterDadosAsync(dataInicio, dataFim);
    }
}