using CestaBasica.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CestaBasica.Web.Pages.Dashboard.RetiradaCesta;

public partial class RetiradaCesta
{
    [Inject]
    private HttpClient Http { get; set; } = default!;

    private string codigoBarras = "";
    private string buscaManual = "";
    private bool retiradaConfirmada = false;
    private string ultimoFuncionario = "";

    private FuncionarioDto? funcionarioSelecionado;

    private List<FuncionarioDto> resultadosBusca = new();
    private List<FuncionarioDto> historico = new();

    protected override async Task OnInitializedAsync()
{
    try
    {
        historico = await Http.GetFromJsonAsync<List<FuncionarioDto>>("api/retiradas/recentes")
                   ?? new();
    }
    catch
    {
        historico = new();
    }
}

    private async Task BuscarPorCodigo(ChangeEventArgs e)
    {
        codigoBarras = e.Value?.ToString() ?? "";

        if (string.IsNullOrWhiteSpace(codigoBarras))
        {
            funcionarioSelecionado = null;
            return;
        }

        funcionarioSelecionado =
            await Http.GetFromJsonAsync<FuncionarioDto>($"api/funcionarios/codigo/{codigoBarras}");

        retiradaConfirmada = false;
    }

    private async Task BuscarManual(ChangeEventArgs e)
    {
        buscaManual = e.Value?.ToString() ?? "";

        if (buscaManual.Length < 2)
        {
            resultadosBusca.Clear();
            return;
        }

        resultadosBusca =
            await Http.GetFromJsonAsync<List<FuncionarioDto>>($"api/funcionarios/buscar?termo={buscaManual}")
            ?? new();
    }

    private void SelecionarFuncionario(FuncionarioDto funcionario)
    {
        funcionarioSelecionado = funcionario;
        resultadosBusca.Clear();
        buscaManual = funcionario.NomeCompleto;
        retiradaConfirmada = false;
    }

    private async Task ConfirmarRetirada()
    {
        if (funcionarioSelecionado is null)
            return;

        var response = await Http.PostAsync(
            $"api/retiradas/confirmar/{funcionarioSelecionado.Id}",
            null
        );

        if (!response.IsSuccessStatusCode)
            return;

        ultimoFuncionario = funcionarioSelecionado.NomeCompleto;
        funcionarioSelecionado.Status = "Retirado";

        historico = await Http.GetFromJsonAsync<List<FuncionarioDto>>("api/retiradas/recentes")
                   ?? new();

        retiradaConfirmada = true;
    }
}