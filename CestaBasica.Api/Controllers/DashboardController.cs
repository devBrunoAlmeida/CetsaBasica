using CestaBasica.Api.Applications;
using Microsoft.AspNetCore.Mvc;

namespace CestaBasica.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly DashboardApplication _app;

    public DashboardController(DashboardApplication app)
    {
        _app = app;
    }

    [HttpGet]
    public async Task<IActionResult> ObterDados(
    [FromQuery] DateTime? dataInicio,
    [FromQuery] DateTime? dataFim)
    {
        try
        {
            var result = await _app.ObterDadosAsync(dataInicio, dataFim);
            return Ok(result);
        }
        catch (ArgumentException ex)
    {
        return BadRequest(new
        {
            mensagem = ex.Message
        });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new
        {
            mensagem = "Erro interno ao carregar dashboard.",
            detalhe = ex.Message
        });
    }
    }


}