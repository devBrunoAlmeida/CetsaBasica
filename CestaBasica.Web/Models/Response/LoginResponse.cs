using CestaBasica.Shared.Enums;

namespace CestaBasica.Web.Models.Responses;

public class LoginResponse
{
    public string Mensagem { get; set; } = string.Empty;

    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Perfil { get; set; } = string.Empty;
}