using CestaBasica.Api.Applications;
using Microsoft.AspNetCore.Mvc;

namespace CestaBasica.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportacaoController : ControllerBase
{
    private readonly ImportacaoApplication _app;

    public ImportacaoController(ImportacaoApplication app)
    {
        _app = app;
    }

    [HttpPost("funcionarios")]
    public async Task<IActionResult> ImportarFuncionarios(IFormFile arquivo)
    {
        if (arquivo == null || arquivo.Length == 0)
            return BadRequest("Arquivo inválido.");

        var resultado = await _app.ImportarFuncionariosAsync(arquivo);

        return Ok(resultado);
    }

    [HttpGet("historico")]
    public async Task<IActionResult> ObterHistorico()
    {
        var resultado = await _app.ObterHistoricoAsync();
        return Ok(resultado);
    }
}