using CestaBasica.Api.DTOs;
using CestaBasica.Api.Repositories;

namespace CestaBasica.Api.Services;

public class DashboardService
{
    private readonly DashboardRepository _repo;

    public DashboardService(DashboardRepository repo)
    {
        _repo = repo;
    }

    public async Task<DashboardDto> ObterDadosAsync()
    {
        return await _repo.ObterDadosAsync();
    }
}