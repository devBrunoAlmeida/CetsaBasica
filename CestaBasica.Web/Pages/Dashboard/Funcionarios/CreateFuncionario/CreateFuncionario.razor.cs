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

    protected async Task Salvar()
{
    var response = await Http.PostAsJsonAsync(
        "api/funcionarios",
        funcionario);

    if (response.IsSuccessStatusCode)
    {
        Navigation.NavigateTo("/funcionarios");
    }
}

    protected void Cancelar()
    {
        Navigation.NavigateTo("/funcionarios");
    }

    protected void FormatarTelefone(ChangeEventArgs e)
{
    var numeros = new string((e.Value?.ToString() ?? "")
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