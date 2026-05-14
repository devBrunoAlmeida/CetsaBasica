using CestaBasica.Api.Applications;
using CestaBasica.Api.DTOs;
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

    [HttpPost]
    public async Task<IActionResult> Registrar(RetiradaRequestDto dto)
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
    
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var result = await _app.ListarAsync();
        return Ok(result);
    }
}