using CestaBasica.Api.Enums;

namespace CestaBasica.Api.DTOs;

public class UsuarioCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; }
}