using System.Net.Http.Json;
using CestaBasica.Shared.DTOs;
using CestaBasica.Web.Models.Requests;
using CestaBasica.Web.Models.Responses;

namespace CestaBasica.Web.Services;

public class UsuarioApiService
{
    private readonly HttpClient _http;

    public UsuarioApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<(LoginResponse? Usuario, string? Erro)> LoginComDetalheAsync(LoginRequest request)
    {
        var response = await _http.PostAsJsonAsync(
            "api/usuarios/login",
            request
        );

        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var erro = await response.Content.ReadFromJsonAsync<ErroApiDto>();
                return (null, erro?.Mensagem ?? "Usuário ou senha inválidos.");
            }
            catch
            {
                return (null, "Usuário ou senha inválidos.");
            }
        }

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return (loginResponse, null);
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var (usuario, _) = await LoginComDetalheAsync(request);
        return usuario;
    }
}
