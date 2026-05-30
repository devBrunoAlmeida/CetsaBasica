using CestaBasica.Web.Models.Responses;
using CestaBasica.Shared.Enums;

namespace CestaBasica.Web.Services;

public class SessaoUsuarioService
{
    public LoginResponse? UsuarioLogado { get; private set; }

    public bool EstaLogado => UsuarioLogado != null;

    public bool IsAdmin =>
    UsuarioLogado?.Perfil == "Admin";

    public bool IsUsuario =>
        UsuarioLogado?.Perfil == "Usuario";

    public void Login(LoginResponse usuario)
    {
        UsuarioLogado = usuario;
    }

    public void Logout()
    {
        UsuarioLogado = null;
    }
}