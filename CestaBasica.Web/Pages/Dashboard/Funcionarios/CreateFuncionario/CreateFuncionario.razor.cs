using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using CestaBasica.Shared.DTOs;

namespace CestaBasica.Web.Pages.Dashboard.Funcionarios.CreateFuncionario;

public partial class CreateFuncionario : ComponentBase
{
    [Inject]
    private HttpClient Http { get; set; } = default!;
    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    protected FuncionarioDto funcionario = new();
    protected List<string> setores = new();
    protected string setorSelecionado = string.Empty;
    protected string novoSetor = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        setores = await Http.GetFromJsonAsync<List<string>>("api/funcionarios/setores")
                  ?? new List<string>();

        funcionario.Setor = string.Empty;
    }

    private void AoSelecionarSetor()
    {
        if (string.IsNullOrWhiteSpace(setorSelecionado))
        {
            funcionario.Setor = string.Empty;
            return;
        }

        if (setorSelecionado == "Outro")
        {
            funcionario.Setor = novoSetor;
            return;
        }

        funcionario.Setor = setorSelecionado;
    }

    protected void AtualizarNovoSetor()
    {
        funcionario.Setor = novoSetor;
    }
    private async Task Salvar()
    {
        if (setorSelecionado == "Outro")
        {
            funcionario.Setor = novoSetor;
        }

        if (string.IsNullOrWhiteSpace(funcionario.Setor))
        {
            return;
        }

        var response = await Http.PostAsJsonAsync("api/funcionarios", funcionario);

        if (response.IsSuccessStatusCode)
        {
            funcionario = new FuncionarioDto
            {
                Setor = string.Empty
            };

            setorSelecionado = string.Empty;
            novoSetor = string.Empty;

            StateHasChanged();
        }
    }

    protected void Cancelar()
    {
        Navigation.NavigateTo("/funcionarios");
    }

    protected void FormatarTelefone(string? valor)
    {
        var numeros = new string((valor ?? "")
            .Where(char.IsDigit)
            .ToArray());

        if (numeros.Length > 11)
            numeros = numeros[..11];

        funcionario.Telefone = numeros.Length <= 2
            ? numeros
            : numeros.Length <= 6
                ? $"({numeros[..2]}) {numeros[2..]}"
                : numeros.Length <= 10
                    ? $"({numeros[..2]}) {numeros[2..6]}-{numeros[6..]}"
                    : $"({numeros[..2]}) {numeros[2..7]}-{numeros[7..]}";
    }
}