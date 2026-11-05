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
            FuncionarioNome = x.Funcionario?.NomeCompleto ?? $"ID #{x.FuncionarioId}",
            Canal = x.Canal,
            Titulo = x.Titulo,
            Mensagem = x.Mensagem,
            Status = x.Status,
            DataEnvio = x.DataEnvio
        }).ToList();
    }

    public Task<WhatsappStatusDto> ObterStatusWhatsappAsync()
        => _service.ObterStatusWhatsappAsync();

    public Task<WhatsappStatusDto> ConfigurarWhatsappAsync(WhatsappConfigDto dto)
        => _service.ConfigurarWhatsappAsync(dto);

    public Task<WhatsappConexaoResultadoDto> ConectarWhatsappAsync(WhatsappConectarRequestDto dto)
        => _service.ConectarWhatsappAsync(dto);

    public Task<WhatsappStatusDto> DesconectarWhatsappAsync()
        => _service.DesconectarWhatsappAsync();

    public Task<string> EnviarTesteWhatsappAsync(WhatsappTesteEnvioDto dto)
        => _service.EnviarTesteWhatsappAsync(dto);
}
