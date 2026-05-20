using CestaBasica.Api.Models;
using CestaBasica.Api.Services;
using CestaBasica.Shared.DTOs;

namespace CestaBasica.Api.Applications;

public class UsuarioApplication
{
    private readonly UsuarioService _service;

    public UsuarioApplication(UsuarioService service)
    {
        _service = service;
    }

    public async Task<Usuario> CriarAsync(UsuarioCreateDto dto)
    {
        return await _service.CriarAsync(dto);
    }

    public async Task<List<Usuario>> ListarAsync()
    {
        return await _service.ListarAsync();
    }

    public async Task<Usuario> LoginAsync(LoginDto dto)
    {
        return await _service.LoginAsync(dto);
    }
}