using CestaBasica.Api.DTOs;
using CestaBasica.Api.Models;
using CestaBasica.Api.Services;

namespace CestaBasica.Api.Applications;

public class NotificacaoApplication
{
    private readonly NotificacaoService _service;

    public NotificacaoApplication(NotificacaoService service)
    {
        _service = service;
    }

    public async Task<Notificacao> EnviarAsync(NotificacaoRequestDto dto)
    {
        return await _service.EnviarAsync(dto);
    }

    public async Task<List<Notificacao>> ListarAsync()
    {
        return await _service.ListarAsync();
    }
}