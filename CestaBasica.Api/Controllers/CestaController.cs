using CestaBasica.Api.Applications;
using CestaBasica.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CestaBasica.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CestasController : ControllerBase
{
    private readonly CestaApplication _app;

    public CestasController(CestaApplication app)
    {
        _app = app;
    }

    [HttpPost]
    public async Task<IActionResult> Criar(CestaDto dto)
    {
        try
        {
            var result = await _app.CriarAsync(dto);
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