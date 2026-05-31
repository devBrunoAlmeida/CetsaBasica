using CestaBasica.Api.Applications;
using CestaBasica.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CestaBasica.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfiguracoesController : ControllerBase
{
    private readonly ConfiguracoesApplication _app;

    public ConfiguracoesController(ConfiguracoesApplication app)
    {
        _app = app;
    }

    [HttpGet]
    public async Task<IActionResult> Obter()
    {
        var result = await _app.ObterAsync();
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Atualizar(ConfiguracoesDto dto)
    {
        var result = await _app.AtualizarAsync(dto);
        return Ok(result);
    }
}