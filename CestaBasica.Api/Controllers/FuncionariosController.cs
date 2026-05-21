using Microsoft.AspNetCore.Mvc;
using CestaBasica.Api.Applications;
using CestaBasica.Shared.DTOs;

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
    public async Task<IActionResult> Criar(FuncionarioDto dto)
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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        await _app.ExcluirAsync(id);
        return NoContent();
    }
}