using CestaBasica.Api.Models;
using CestaBasica.Api.Services;
using CestaBasica.Shared.DTOs;

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

    public async Task<List<NotificacaoRequestDto>> ListarAsync()
    {
        var notificacoes = await _service.ListarAsync();

        return notificacoes.Select(x => new NotificacaoRequestDto
        {
            FuncionarioId = x.FuncionarioId,
            Canal = x.Canal,
            Titulo = x.Titulo,
            Mensagem = x.Mensagem,
            DataEnvio = x.DataEnvio
        }).ToList();
    }
}