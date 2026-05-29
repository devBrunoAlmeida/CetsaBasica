using System.Net.Http.Json;
using CestaBasica.Shared.DTOs;
using Microsoft.AspNetCore.Components;

namespace CestaBasica.Web.Pages.Dashboard.EstoqueCesta;

public partial class Cesta : ComponentBase
{
    [Inject]
    private HttpClient Http { get; set; } = default!;

    protected List<CestaDto> cestas = [];

    protected CestaDto novaCesta = new();

    protected override async Task OnInitializedAsync()
    {
        await CarregarCestas();
    }

    private async Task CarregarCestas()
    {
        cestas = await Http.GetFromJsonAsync<List<CestaDto>>("api/cestas") ?? [];
    }

    private async Task CadastrarCesta()
    {
        novaCesta.Ativa = true;

        var response = await Http.PostAsJsonAsync("api/cestas", novaCesta);

        if (response.IsSuccessStatusCode)
        {
            novaCesta = new CestaDto();
            await CarregarCestas();
        }
    }

    private async Task DesativarCesta(int id)
    {
        var response = await Http.PutAsync($"api/cestas/{id}/desativar", null);

        if (response.IsSuccessStatusCode)
        {
            await CarregarCestas();
        }
    }
    private async Task EditarCesta(int id)
    {
        var response = await Http.PutAsync($"api/cestas/{id}/editar", null);

        if (response.IsSuccessStatusCode)
        {
            await CarregarCestas();
        }
    }
}