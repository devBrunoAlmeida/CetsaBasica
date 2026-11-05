using System.Net.Http.Json;
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
    private string? erroRetirada;
    private string ultimoFuncionario = "";

    private FuncionarioDto? funcionarioSelecionado;
    private List<FuncionarioDto> resultadosBusca = new();
    private List<FuncionarioDto> historico = new();

    private List<RetiradaRecenteDto> historicoRetiradas = new();

    protected override async Task OnInitializedAsync()
    {
        await CarregarHistoricoFuncionarios();
        await CarregarHistoricoRetiradas();
    }

    private async Task CarregarHistoricoFuncionarios()
    {
        try
        {
            historico = await Http.GetFromJsonAsync<List<FuncionarioDto>>(
                "api/retiradas/recentes")
                ?? new();
        }
        catch
        {
            historico = new();
        }
    }

    private async Task CarregarHistoricoRetiradas()
    {
        try
        {
            historicoRetiradas = await Http.GetFromJsonAsync<List<RetiradaRecenteDto>>(
                "api/retiradas/recentes-detalhado")
                ?? new();
        }
        catch
        {
            historicoRetiradas = new();
        }
    }

    private async Task BuscarPorCodigo(ChangeEventArgs e)
    {
        codigoBarras = e.Value?.ToString()?.Trim() ?? "";
        erroRetirada = null;
        retiradaConfirmada = false;

        if (string.IsNullOrWhiteSpace(codigoBarras))
        {
            funcionarioSelecionado = null;
            return;
        }

        try
        {
            var response = await Http.GetAsync($"api/funcionarios/codigo/{Uri.EscapeDataString(codigoBarras)}");
            if (response.IsSuccessStatusCode)
            {
                funcionarioSelecionado = await response.Content.ReadFromJsonAsync<FuncionarioDto>();
            }
            else
            {
                funcionarioSelecionado = null;
            }
        }
        catch
        {
            funcionarioSelecionado = null;
        }
    }

    private async Task BuscarManual(ChangeEventArgs e)
    {
        buscaManual = e.Value?.ToString() ?? "";
        erroRetirada = null;

        if (buscaManual.Length < 2)
        {
            resultadosBusca.Clear();
            return;
        }

        try
        {
            resultadosBusca =
                await Http.GetFromJsonAsync<List<FuncionarioDto>>($"api/funcionarios/buscar?termo={Uri.EscapeDataString(buscaManual)}")
                ?? new();
        }
        catch
        {
            resultadosBusca.Clear();
        }
    }

    private void SelecionarFuncionario(FuncionarioDto funcionario)
    {
        funcionarioSelecionado = funcionario;
        resultadosBusca.Clear();
        buscaManual = funcionario.NomeCompleto;
        retiradaConfirmada = false;
        erroRetirada = null;
    }

    private async Task ConfirmarRetirada()
    {
        if (funcionarioSelecionado is null)
            return;

        erroRetirada = null;
        retiradaConfirmada = false;

        var response = await Http.PostAsync(
            $"api/retiradas/confirmar/{funcionarioSelecionado.Id}",
            null
        );

        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var erro = await response.Content.ReadFromJsonAsync<ErroApiDto>();
                erroRetirada = erro?.Mensagem ?? "Não foi possível registrar a retirada.";
            }
            catch
            {
                erroRetirada = "Não foi possível registrar a retirada.";
            }
            return;
        }

        ultimoFuncionario = funcionarioSelecionado.NomeCompleto;
        funcionarioSelecionado.Status = "Retirado";

        await CarregarHistoricoFuncionarios();
        await CarregarHistoricoRetiradas();

        retiradaConfirmada = true;
    }
}
