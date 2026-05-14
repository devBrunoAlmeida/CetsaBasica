using CestaBasica.Api.DTOs;
using CestaBasica.Api.Models;
using CestaBasica.Api.Services;

namespace CestaBasica.Api.Applications;

public class FuncionarioApplication
{
    private readonly FuncionarioService _service;

    public FuncionarioApplication(FuncionarioService service)
    {
        _service = service;
    }

    public async Task<Funcionario> CriarAsync(FuncionarioCreateDto dto)
    {
        return await _service.CriarAsync(dto);
    }

    public async Task<List<Funcionario>> ListarAsync()
    {
        return await _service.ListarAsync();
    }
}