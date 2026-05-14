using CestaBasica.Api.DTOs;
using CestaBasica.Api.Services;

namespace CestaBasica.Api.Applications;

public class DashboardApplication
{
    private readonly DashboardService _service;

    public DashboardApplication(DashboardService service)
    {
        _service = service;
    }

    public async Task<DashboardDto> ObterDadosAsync()
    {
        return await _service.ObterDadosAsync();
    }
}