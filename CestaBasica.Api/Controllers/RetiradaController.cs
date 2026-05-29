using CestaBasica.Api.Applications;
using CestaBasica.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CestaBasica.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RetiradasController : ControllerBase
{
    private readonly RetiradaApplication _app;

    public RetiradasController(RetiradaApplication app)
    {
        _app = app;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar(RetiradaCestaDto dto)
    {
        try
        {
            var result = await _app.RegistrarRetiradaAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("listar")]
    public async Task<IActionResult> Listar()
    {
        var result = await _app.ListarAsync();
        return Ok(result);
    }

    [HttpGet("recentes")]
    public async Task<IActionResult> ListarRecentes()
    {
        var result = await _app.ListarRecentesAsync();
        return Ok(result);
    }

    [HttpPost("confirmar/{funcionarioId:int}")]
    public async Task<IActionResult> ConfirmarRetirada(int funcionarioId)
    {
        try
        {
            await _app.ConfirmarRetiradaAsync(funcionarioId);
            return Ok(new { mensagem = "Retirada registrada com sucesso." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}