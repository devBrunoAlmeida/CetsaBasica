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
    public async Task<IActionResult> ObterDados()
    {
        var result = await _app.ObterDadosAsync();
        return Ok(result);
    }
}