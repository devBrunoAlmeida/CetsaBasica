using Microsoft.AspNetCore.Mvc;
using CestaBasica.Api.Applications;
using CestaBasica.Api.DTOs;

namespace CestaBasica.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FuncionariosController : ControllerBase
{
    private readonly FuncionarioApplication _app;

    public FuncionariosController(FuncionarioApplication app)
    {
        _app = app;
    }

    [HttpPost]
    public async Task<IActionResult> Criar(FuncionarioCreateDto dto)
    {
        var result = await _app.CriarAsync(dto);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var result = await _app.ListarAsync();
        return Ok(result);
    }
}