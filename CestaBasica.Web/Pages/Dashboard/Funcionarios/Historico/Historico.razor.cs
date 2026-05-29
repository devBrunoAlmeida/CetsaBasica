using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using CestaBasica.Shared.DTOs;

namespace CestaBasica.Web.Pages.Dashboard.Funcionarios.Historico;

public partial class Historico : ComponentBase
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    [Parameter] public int Id { get; set; }

    protected FuncionarioDto? Funcionario { get; set; }
    protected HistoricoFuncionarioDto? HistoricoFuncionario { get; set; }
    protected List<EventoHistoricoDto> Eventos { get; set; } = new();

    protected IEnumerable<EventoHistoricoDto> Retiradas =>
        Eventos.Where(x => x.Tipo == "RETIRADA");

    protected IEnumerable<EventoHistoricoDto> Notificacoes =>
        Eventos.Where(x => x.Tipo == "NOTIFICACAO");

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Funcionario = await Http.GetFromJsonAsync<FuncionarioDto>(
                $"api/funcionarios/{Id}");

            HistoricoFuncionario = await Http.GetFromJsonAsync<HistoricoFuncionarioDto>(
                $"api/funcionarios/{Id}/historico");
        }
        catch
        {
            Funcionario = null;
            HistoricoFuncionario = null;
            Eventos = new();
        }
    }

    protected void Voltar()
    {
        Navigation.NavigateTo("/funcionarios");
    }

    protected string ObterClasseTimeline(string tipo)
    {
        return tipo switch
        {
            "RETIRADA" => "success",
            "NOTIFICACAO" => "delivered",
            "CADASTRO" => "info",
            _ => "info"
        };
    }

    protected string ObterIniciais(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return "--";

        var partes = nome.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (partes.Length == 1)
            return partes[0][..Math.Min(2, partes[0].Length)].ToUpper();

        return $"{partes[0][0]}{partes[^1][0]}".ToUpper();
    }
}