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

    public async Task<List<FuncionarioDto>> ListarRecentesAsync()
    {
        return await _service.ListarRecentesAsync();
    }

    public async Task ConfirmarRetiradaAsync(int funcionarioId)
    {
        await _service.ConfirmarRetiradaAsync(funcionarioId);
    }
    public async Task<List<RetiradaRecenteDto>> ListarRecentesDetalhadoAsync()
{
    return await _service.ListarRecentesDetalhadoAsync();
}
}