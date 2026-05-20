using CestaBasica.Shared.DTOs;
using CestaBasica.Api.Services;

namespace CestaBasica.Api.Applications;

public class DashboardApplication
{
    private readonly DashboardService _service;

    public DashboardApplication(DashboardService service)
    {
        _service = service;
    }

    public async Task<DashboardDto> ObterDadosAsync(DateTime? dataInicio, DateTime? dataFim)
    {
        return await _service.ObterDadosAsync(dataInicio, dataFim);
    }
}