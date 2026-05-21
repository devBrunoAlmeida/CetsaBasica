using System.Net.Http.Json;
using CestaBasica.Shared.DTOs;
using Microsoft.AspNetCore.Components;



namespace CestaBasica.Web.Pages.Dashboard.Funcionarios;
public partial class Funcionarios : ComponentBase
{
    [Inject]
    private HttpClient Http { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    protected List<FuncionarioDto> funcionarios = [];

    protected override async Task OnInitializedAsync()
    {
        funcionarios =
            await Http.GetFromJsonAsync<List<FuncionarioDto>>
            ("api/funcionarios")
            ?? [];
    }

    private void EditarFuncionario(int id)
    {
        Navigation.NavigateTo($"/funcionarios/editar/{id}");
    }

    private async Task ExcluirFuncionario(int id)
    {
        var response =
            await Http.DeleteAsync($"api/funcionarios/{id}");

        if (response.IsSuccessStatusCode)
        {
            funcionarios.RemoveAll(x => x.Id == id);
        }
    }

    private void VisualizarHistorico(int id)
    {
        Navigation.NavigateTo($"/funcionarios/historico/{id}");
    }

    protected string termoBusca = string.Empty;

    protected IEnumerable<FuncionarioDto> FuncionariosFiltrados =>
        string.IsNullOrWhiteSpace(termoBusca)
            ? funcionarios
            : funcionarios.Where(f =>
                f.Nome.Contains(termoBusca, StringComparison.OrdinalIgnoreCase) ||
                f.Matricula.Contains(termoBusca, StringComparison.OrdinalIgnoreCase) ||
                f.CodigoBarras.Contains(termoBusca, StringComparison.OrdinalIgnoreCase)
            );

    protected int TotalFuncionarios => FuncionariosFiltrados.Count();
}