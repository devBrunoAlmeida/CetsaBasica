using System.Net.Http.Json;
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

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var response = await _http.PostAsJsonAsync(
            "api/usuarios/login",
            request
        );

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content
            .ReadFromJsonAsync<LoginResponse>();
    }
}