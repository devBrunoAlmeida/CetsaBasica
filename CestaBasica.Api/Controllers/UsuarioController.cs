using CestaBasica.Api.Applications;
using CestaBasica.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CestaBasica.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioApplication _app;

    public UsuariosController(UsuarioApplication app)
    {
        _app = app;
    }

    [HttpPost]
    public async Task<IActionResult> Criar(UsuarioCreateDto dto)
    {
        try
        {
            var result = await _app.CriarAsync(dto);

            return Ok(new
            {
                result.Id,
                result.Nome,
                result.Email,
                Perfil = result.Perfil.ToString(),
                result.Ativo
            });
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

        return Ok(result.Select(u => new
        {
            u.Id,
            u.Nome,
            u.Email,
            Perfil = u.Perfil.ToString(),
            u.Ativo
        }));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        try
        {
            var usuario = await _app.LoginAsync(dto);

            return Ok(new
            {
                mensagem = "Login realizado com sucesso.",
                usuario.Id,
                usuario.Nome,
                usuario.Email,
                Perfil = usuario.Perfil.ToString()
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}