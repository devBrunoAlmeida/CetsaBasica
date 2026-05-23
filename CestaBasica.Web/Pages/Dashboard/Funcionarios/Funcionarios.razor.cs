using System.Net.Http.Json;
using CestaBasica.Shared.DTOs;
using Microsoft.AspNetCore.Components;



namespace CestaBasica.Web.Pages.Dashboard.Funcionarios;

public partial class Funcionarios : ComponentBase
{
    [Inject]
    private HttpClient Http { get; set; } = default!;

    protected List<FuncionarioDto> funcionarios = [];
    protected int? EditarFuncionarioId;

    protected override async Task OnInitializedAsync()
    {
        funcionarios =
            await Http.GetFromJsonAsync<List<FuncionarioDto>>
            ("api/funcionarios")
            ?? [];
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

    protected bool MostrarHistorico;
    protected FuncionarioDto? FuncionarioHistorico;

    private void VisualizarHistorico(FuncionarioDto funcionario)
    {
        FuncionarioHistorico = funcionario;
        MostrarHistorico = true;
    }

    private void FecharHistorico()
    {
        MostrarHistorico = false;
        FuncionarioHistorico = null;
    }
    protected int PaginaAtual = 1;
    protected int ItensPorPagina = 20;

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