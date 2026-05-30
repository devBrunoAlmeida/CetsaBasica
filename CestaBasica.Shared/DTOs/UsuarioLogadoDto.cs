
using CestaBasica.Shared.Enums;

namespace CestaBasica.Shared.DTOs;

public class UsuarioLogadoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; }
}