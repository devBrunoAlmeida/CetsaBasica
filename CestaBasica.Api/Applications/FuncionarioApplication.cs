using CestaBasica.Shared.DTOs;
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

    public async Task<FuncionarioDto> CriarAsync(FuncionarioDto dto)
    {
        return await _service.CriarAsync(dto);
    }

    public async Task<List<FuncionarioDto>> ListarAsync()
    {
        return await _service.ListarAsync();
    }

    public async Task<Funcionario> AtualizarAsync(FuncionarioDto dto)
    {
        return await _service.AtualizarAsync(dto);
    }

    public async Task ExcluirAsync(int id)
    {
        await _service.ExcluirAsync(id);
    }
    public async Task<HistoricoFuncionarioDto> ObterHistoricoAsync(int funcionarioId)
    {
        return await _service.ObterHistoricoAsync(funcionarioId);
    }
    public async Task<FuncionarioDto?> BuscarPorCodigoAsync(string codigoBarras)
    {
        return await _service.BuscarPorCodigoAsync(codigoBarras);
    }

    public async Task<List<FuncionarioDto>> BuscarManualAsync(string termo)
    {
        return await _service.BuscarManualAsync(termo);
    }
    public async Task<FuncionarioDto?> BuscarPorIdAsync(int id)
{
    return await _service.BuscarPorIdAsync(id);
}
}