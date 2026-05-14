using CestaBasica.Api.DTOs;
using CestaBasica.Api.Models;
using CestaBasica.Api.Services;

namespace CestaBasica.Api.Applications;

public class CestaApplication
{
    private readonly CestaService _service;

    public CestaApplication(CestaService service)
    {
        _service = service;
    }

    public async Task<Cesta> CriarAsync(CestaCreateDto dto)
    {
        return await _service.CriarAsync(dto);
    }

    public async Task<List<Cesta>> ListarAsync()
    {
        return await _service.ListarAsync();
    }
}