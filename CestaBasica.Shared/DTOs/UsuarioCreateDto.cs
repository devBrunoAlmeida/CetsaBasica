using CestaBasica.Shared.Enums;

namespace CestaBasica.Shared.DTOs;

public class UsuarioCreateDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; }
    public bool Ativo { get; set; } = true;
}

public class UsuarioUpdateDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? NovaSenha { get; set; }
    public PerfilUsuario Perfil { get; set; }
    public bool Ativo { get; set; } = true;
}

public class UsuarioListItemDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
    public bool Ativo { get; set; }
}