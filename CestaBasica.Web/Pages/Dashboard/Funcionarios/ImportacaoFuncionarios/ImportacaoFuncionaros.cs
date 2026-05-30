using CestaBasica.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Web;

namespace CestaBasica.Web.Pages.Dashboard.Funcionarios.ImportacaoFuncionarios;

public partial class ImportacaoFuncionarios : ComponentBase
{
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;
    [Inject] private HttpClient Http { get; set; } = default!;

    private string nomeArquivo = "";
    private int progresso = 0;
    private bool importando = false;
    private IBrowserFile? arquivoSelecionado;

    private List<RegistroImportacaoDto> registros = new();

    private List<HistoricoImportacaoDto> historico = new();

    private int validos => registros.Count(x => x.Valido);
    private int invalidos => registros.Count(x => !x.Valido);
    private int total => registros.Count;

    protected override async Task OnInitializedAsync()
{
    await CarregarHistoricoAsync();
}
private async Task CarregarHistoricoAsync()
{
    historico = await Http.GetFromJsonAsync<List<HistoricoImportacaoDto>>(
        "api/importacao/historico"
    ) ?? new();
}


    private void SelecionarArquivo(InputFileChangeEventArgs e)
    {
        arquivoSelecionado = e.File;
        nomeArquivo = e.File.Name;
        progresso = 0;
        registros = new();
    }
    private void RemoverArquivo()
    {
        arquivoSelecionado = null;
        nomeArquivo = "";
        progresso = 0;
        registros = new();
    }

    private async Task ImportarRegistrosValidos()
    {
        if (arquivoSelecionado is null) return;

        importando = true;
        progresso = 10;
        StateHasChanged();

        try
        {
            await using var stream = arquivoSelecionado.OpenReadStream(20_000_000);

            using var content = new MultipartFormDataContent();
            using var fileContent = new StreamContent(stream);

            content.Add(fileContent, "arquivo", arquivoSelecionado.Name);

            progresso = 40;
            StateHasChanged();

            var response = await Http.PostAsync("api/importacao/funcionarios", content);

            progresso = 80;
            StateHasChanged();

            if (response.IsSuccessStatusCode)
            {
                var resultado = await response.Content
                    .ReadFromJsonAsync<ImportacaoResultadoDto>();

                if (resultado is not null)
{
    registros = resultado.Registros;
    await CarregarHistoricoAsync();
}
            }
            else
            {
                var erro = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Erro {response.StatusCode}: {erro}");
            }

            progresso = 100;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exceção: {ex.Message}");
        }
        finally
        {
            // ✅ reseta o arquivo após importar
            arquivoSelecionado = null;
            nomeArquivo = "";
            importando = false;
            StateHasChanged();
        }
        
    }
}