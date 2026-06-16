using System.Net.Http.Json;
using System.Text.Json;

namespace CestaBasica.Api.Services;

public class EvolutionApiService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _configuration;

    public EvolutionApiService(HttpClient http, IConfiguration configuration)
    {
        _http = http;
        _configuration = configuration;
    }

    public async Task<string> EnviarTextoAsync(string telefone, string mensagem)
    {
        var baseUrl = _configuration["EvolutionApi:BaseUrl"];
        var apiKey = _configuration["EvolutionApi:ApiKey"];
        var instance = _configuration["EvolutionApi:Instance"];

        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new Exception("EvolutionApi:BaseUrl não foi lido do appsettings.");

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new Exception("EvolutionApi:ApiKey não foi lido do appsettings.");

        if (string.IsNullOrWhiteSpace(instance))
            throw new Exception("EvolutionApi:Instance não foi lido do appsettings.");

        var numero = new string(telefone.Where(char.IsDigit).ToArray());

        if (!numero.StartsWith("55"))
            numero = "55" + numero;

        var body = new
        {
            number = numero,
            text = mensagem
        };

        var url = $"{baseUrl}/message/sendText/{instance}";

        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("apikey", apiKey);
        request.Content = JsonContent.Create(body);

        var response = await _http.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception(content);

        using var json = JsonDocument.Parse(content);

        return json.RootElement
            .GetProperty("key")
            .GetProperty("id")
            .GetString() ?? content;
    }
}