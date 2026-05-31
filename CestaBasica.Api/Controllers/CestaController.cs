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

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var cestas = await _app.ListarAsync();
        return Ok(cestas);
    }

    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] CestaDto dto)
    {
        await _app.CadastrarAsync(dto);
        return Ok();
    }

    [HttpPut("{id}/desativar")]
    public async Task<IActionResult> Desativar(int id)
    {
        await _app.DesativarAsync(id);
        return Ok();
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] CestaDto dto)
    {
        dto.Id = id;

        await _app.AtualizarAsync(dto);

        return Ok();
    }
    [HttpPut("{id}/ativar")]
    public async Task<IActionResult> Ativar(int id)
    {
        await _app.AtivarAsync(id);
        return Ok();
    }
}