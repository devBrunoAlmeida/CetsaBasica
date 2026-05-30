using CestaBasica.Shared.Enums;

namespace CestaBasica.Shared.DTOs;

public class UsuarioCreateDto
{
    public string Id { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; }
}