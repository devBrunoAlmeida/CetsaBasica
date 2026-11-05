using CestaBasica.Api.Applications;
using CestaBasica.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CestaBasica.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificacoesController : ControllerBase
{
    private readonly NotificacaoApplication _app;

    public NotificacoesController(NotificacaoApplication app)
    {
        _app = app;
    }

    [HttpPost("enviar")]
    public async Task<IActionResult> Enviar(NotificacaoRequestDto dto)
    {
        try
        {
            var result = await _app.EnviarAsync(dto);
            return Ok(new
            {
                result.Id,
                result.FuncionarioId,
                result.Canal,
                result.Status,
                result.ProtocoloExterno,
                result.Erro,
                result.DataEnvio
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var result = await _app.ListarAsync();
        return Ok(result);
    }

    [HttpGet("whatsapp/status")]
    public async Task<IActionResult> ObterStatusWhatsapp()
    {
        var status = await _app.ObterStatusWhatsappAsync();
        return Ok(status);
    }

    [HttpPut("whatsapp/configurar")]
    public async Task<IActionResult> ConfigurarWhatsapp([FromBody] WhatsappConfigDto dto)
    {
        try
        {
            var status = await _app.ConfigurarWhatsappAsync(dto);
            return Ok(status);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("whatsapp/conectar")]
    public async Task<IActionResult> ConectarWhatsapp([FromBody] WhatsappConectarRequestDto dto)
    {
        try
        {
            var resultado = await _app.ConectarWhatsappAsync(dto);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("whatsapp/desconectar")]
    public async Task<IActionResult> DesconectarWhatsapp()
    {
        try
        {
            var status = await _app.DesconectarWhatsappAsync();
            return Ok(status);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("whatsapp/teste")]
    public async Task<IActionResult> EnviarTesteWhatsapp([FromBody] WhatsappTesteEnvioDto dto)
    {
        try
        {
            var protocolo = await _app.EnviarTesteWhatsappAsync(dto);
            return Ok(new { mensagem = "Mensagem de teste enviada com sucesso!", protocolo });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
