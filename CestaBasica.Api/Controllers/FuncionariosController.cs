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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, FuncionarioDto dto)
    {
        dto.Id = id;

        var result = await _app.AtualizarAsync(dto);

        return Ok(result);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var funcionario = await _app.BuscarPorIdAsync(id);

        if (funcionario is null)
            return NotFound();

        return Ok(funcionario);
    }

    [HttpGet("{id:int}/historico")]
    public async Task<IActionResult> ObterHistorico(int id)
    {
        var result = await _app.ObterHistoricoAsync(id);
        return Ok(result);
    }

    [HttpGet("buscar")]
    public async Task<IActionResult> BuscarManual([FromQuery] string termo)
    {
        var funcionarios = await _app.BuscarManualAsync(termo);
        return Ok(funcionarios);
    }
    [HttpGet("codigo/{codigoBarras}")]
    public async Task<IActionResult> BuscarPorCodigo(string codigoBarras)
    {
        var funcionario = await _app.BuscarPorCodigoAsync(codigoBarras);

        if (funcionario is null)
            return NotFound();

        return Ok(funcionario);
    }
    [HttpGet("exportar-excel")]
    public async Task<IActionResult> ExportarExcel()
    {
        var arquivo = await _app.ExportarExcelAsync();

        return File(
            arquivo,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "funcionarios.xlsx"
        );
    }

    [HttpGet("setores")]
public async Task<IActionResult> ListarSetores()
{
    var setores = await _app.ListarSetoresAsync();
    return Ok(setores);
}
}