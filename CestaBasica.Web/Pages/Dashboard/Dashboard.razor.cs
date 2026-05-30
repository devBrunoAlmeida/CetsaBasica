using System.Net.Http.Json;
using CestaBasica.Shared.DTOs;
using Microsoft.AspNetCore.Components;


namespace CestaBasica.Web.Pages.Dashboard;

public partial class Dashboard : ComponentBase
{
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;
    [Inject] protected HttpClient Http { get; set; } = null!;
    protected bool menuAberto = true;
    protected DashboardDto? dashboard;
    protected bool carregando = true;
    protected string? mensagemErro;
    protected int segundosRestantes = 5;
    protected DateTime? dataInicio;
    protected DateTime? dataFim;

    protected override async Task OnInitializedAsync()
    {
        await CarregarDashboard();
    }

    private async Task MostrarErro(string mensagem)
    {
        mensagemErro = mensagem;
        segundosRestantes = 5;
        StateHasChanged();

        while (segundosRestantes > 0)
        {
            await Task.Delay(1000);
            segundosRestantes--;
            StateHasChanged();
        }

        mensagemErro = null;
        StateHasChanged();
    }
    protected async Task CarregarDashboard()
    {
        carregando = true;
        mensagemErro = null;

        try
        {
            var url = "api/dashboard";
            var parametros = new List<string>();

            if (dataInicio.HasValue)
                parametros.Add($"dataInicio={dataInicio.Value:yyyy-MM-dd}");

            if (dataFim.HasValue)
                parametros.Add($"dataFim={dataFim.Value:yyyy-MM-dd}");

            if (parametros.Count > 0)
                url += "?" + string.Join("&", parametros);

            Console.WriteLine(url);

            var response = await Http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadFromJsonAsync<ErroApiDto>();
                await MostrarErro(
                    erro?.Mensagem ?? "Erro ao carregar dashboard."
                );
                dashboard = null;
                return;
            }

            dashboard = await response.Content.ReadFromJsonAsync<DashboardDto>();
        }
        catch (Exception ex)
        {
            await MostrarErro(
                ex.Message ?? "Erro ao carregar dashboard."
            );
            dashboard = null;
        }
        finally
        {
            carregando = false;
            StateHasChanged();
        }
    }
    protected void AlternarMenu()
    {
        menuAberto = !menuAberto;
    }

}