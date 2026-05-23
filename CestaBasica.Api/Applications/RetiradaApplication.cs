using CestaBasica.Api.Models;
using CestaBasica.Api.Services;
using CestaBasica.Shared.DTOs;

namespace CestaBasica.Api.Applications;

public class RetiradaApplication
{
    private readonly RetiradaService _service;

    public RetiradaApplication(RetiradaService service)
    {
        _service = service;
    }

    public async Task<Retirada> RegistrarRetiradaAsync(RetiradaCestaDto dto)
    {
        return await _service.RegistrarRetiradaAsync(dto);
    }

    public async Task<List<Retirada>> ListarAsync()
    {
        return await _service.ListarAsync();
    }
}