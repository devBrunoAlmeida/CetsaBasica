using System.Net.Http.Json;
using CestaBasica.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;



namespace CestaBasica.Web.Pages.Dashboard.Funcionarios;

public partial class Funcionarios : ComponentBase
{
    [Inject]
    private HttpClient Http { get; set; } = default!;

    protected List<FuncionarioDto> funcionarios = [];
    protected int? EditarFuncionarioId;

    protected int TotalFuncionarios => FuncionariosFiltrados.Count();

    protected string termoBusca = string.Empty;
    protected string filtroStatus = string.Empty;
    protected string filtroSetor = string.Empty;

    protected DateTime? dataInicial;
    protected DateTime? dataFinal;

    protected int PaginaAtual = 1;
    protected int ItensPorPagina = 20;

    protected List<string> setores = [];
    protected void ResetarPagina()
    {
        PaginaAtual = 1;
    }
    protected void FiltrarDataInicial(ChangeEventArgs e)
    {
        if (DateTime.TryParse(e.Value?.ToString(), out var data))
            dataInicial = data;
        else
            dataInicial = null;

        ResetarPagina();
    }

    protected void FiltrarDataFinal(ChangeEventArgs e)
    {
        if (DateTime.TryParse(e.Value?.ToString(), out var data))
            dataFinal = data;
        else
            dataFinal = null;

        ResetarPagina();
    }

    protected override async Task OnInitializedAsync()
{
    funcionarios =
        await Http.GetFromJsonAsync<List<FuncionarioDto>>
        ("api/funcionarios")
        ?? [];

    setores = funcionarios
        .Where(f => !string.IsNullOrWhiteSpace(f.Setor))
        .Select(f => f.Setor)
        .Distinct()
        .OrderBy(s => s)
        .ToList();
}

    private async Task SalvarFuncionario(FuncionarioDto funcionario)
    {
        var response = await Http.PutAsJsonAsync(
            $"api/funcionarios/{funcionario.Id}",
            funcionario);

        if (response.IsSuccessStatusCode)
        {
            funcionarios = await Http.GetFromJsonAsync<List<FuncionarioDto>>
                ("api/funcionarios")
                ?? [];

            EditarFuncionarioId = null;
        }
    }
    private void CancelarEdicao()
    {
        EditarFuncionarioId = null;
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
    protected IEnumerable<FuncionarioDto> FuncionariosFiltrados =>
     funcionarios.Where(f =>
     {
         var buscaOk = string.IsNullOrWhiteSpace(termoBusca)
             || f.NomeCompleto.Contains(termoBusca, StringComparison.OrdinalIgnoreCase)
             || f.Matricula.Contains(termoBusca, StringComparison.OrdinalIgnoreCase)
             || f.CodigoBarras.Contains(termoBusca, StringComparison.OrdinalIgnoreCase);

         var statusOk = string.IsNullOrWhiteSpace(filtroStatus)
             || f.Status == filtroStatus;

         var setorOk = string.IsNullOrWhiteSpace(filtroSetor)
             || f.Setor == filtroSetor;

         return buscaOk && statusOk && setorOk;
     });


    [Inject]
private NavigationManager Navigation { get; set; } = default!;

private void VisualizarHistorico(FuncionarioDto funcionario)
{
    Navigation.NavigateTo($"/funcionarios/{funcionario.Id}/historico");
}

    protected int TotalPaginas =>
        (int)Math.Ceiling((double)TotalFuncionarios / ItensPorPagina);

    protected IEnumerable<FuncionarioDto> FuncionariosPaginados =>
        FuncionariosFiltrados
            .Skip((PaginaAtual - 1) * ItensPorPagina)
            .Take(ItensPorPagina);

    protected void PaginaAnterior()
    {
        if (PaginaAtual > 1)
            PaginaAtual--;
    }

    protected void ProximaPagina()
    {
        if (PaginaAtual < TotalPaginas)
            PaginaAtual++;
    }

    protected void IrParaPagina(int pagina)
    {
        PaginaAtual = pagina;
    }
}