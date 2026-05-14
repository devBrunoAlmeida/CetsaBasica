using CestaBasica.Api.Applications;
using CestaBasica.Api.DTOs;
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
            return Ok(result);
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
}