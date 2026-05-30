using CestaBasica.Shared.Enums;
using CestaBasica.Web.Models.Requests;
using Microsoft.AspNetCore.Components;
using CestaBasica.Web.Services;

namespace CestaBasica.Web.Pages.Login;

public partial class Login
{
    [Inject]
    private SessaoUsuarioService Sessao { get; set; } = default!;

    private LoginRequest login = new()
    {
        Perfil = PerfilUsuario.Usuario
    };

    private string mensagem = "";

    private async Task FazerLogin()
    {
        mensagem = "";

        var response = await UsuarioService.LoginAsync(login);

        if (response is null)
        {
            mensagem = "Usuário ou senha inválidos.";
            return;
        }

        Sessao.Login(response);

        Navigation.NavigateTo("/dashboard");
    }
}