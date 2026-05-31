using System.Net.Http.Json;
using CestaBasica.Shared.DTOs;
using Microsoft.AspNetCore.Components;

namespace CestaBasica.Web.Pages.Dashboard.Configuracoes;

public partial class Configuracoes : ComponentBase
{
    [Inject] private HttpClient Http { get; set; } = default!;

    protected ConfiguracoesDto config = new();
    protected string mensagem = string.Empty;
    protected bool salvando;

    protected override async Task OnInitializedAsync()
    {
        await CarregarConfiguracoes();
    }

    private async Task CarregarConfiguracoes()
    {
        var result = await Http.GetFromJsonAsync<ConfiguracoesDto>("api/configuracoes");
        if (result is not null)
            config = result;
    }

    protected async Task SalvarConfiguracoes()
    {
        salvando = true;
        mensagem = string.Empty;

        var response = await Http.PutAsJsonAsync("api/configuracoes", config);

        mensagem = response.IsSuccessStatusCode
            ? "Configurações salvas com sucesso."
            : "Erro ao salvar configurações.";

        salvando = false;
    }
}