using CestaBasica.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace CestaBasica.Web.Pages.Dashboard.DispararNotificacao;

public partial class DispararNotificacao : ComponentBase
{
    [Inject] private HttpClient Http { get; set; } = default!;

    protected string Mensagem { get; set; } =
        "Prezado(a), informamos que sua cesta básica está disponível para retirada no setor responsável.";

    protected string? MensagemRetorno { get; set; }

    protected List<FuncionarioNotificacaoViewModel> Funcionarios { get; set; } = new();

    protected List<NotificacaoRequestDto> Historico { get; set; } = new();

    protected int PaginaAtual { get; set; } = 1;

    protected int ItensPorPagina { get; set; } = 10;
    protected int PaginaAtualHistorico = 1;
    protected int ItensPorPaginaHistorico = 10;

    protected int TotalPaginasHistorico =>
        Historico.Count == 0
            ? 1
            : (int)Math.Ceiling((double)Historico.Count / ItensPorPaginaHistorico);

    protected IEnumerable<NotificacaoRequestDto> HistoricoPaginado =>
        Historico
            .OrderByDescending(x => x.DataEnvio)
            .Skip((PaginaAtualHistorico - 1) * ItensPorPaginaHistorico)
            .Take(ItensPorPaginaHistorico);

    private void PaginaAnteriorHistorico()
    {
        if (PaginaAtualHistorico > 1)
            PaginaAtualHistorico--;
    }

    private void ProximaPaginaHistorico()
    {
        if (PaginaAtualHistorico < TotalPaginasHistorico)
            PaginaAtualHistorico++;
    }
    protected int TotalPaginas =>
        Funcionarios.Count == 0
            ? 1
            : (int)Math.Ceiling((double)Funcionarios.Count / ItensPorPagina);

    protected List<FuncionarioNotificacaoViewModel> FuncionariosPaginados =>
        Funcionarios
            .Skip((PaginaAtual - 1) * ItensPorPagina)
            .Take(ItensPorPagina)
            .ToList();

    protected override async Task OnInitializedAsync()
    {
        await CarregarFuncionarios();
        await CarregarHistorico();
    }

    protected async Task CarregarFuncionarios()
    {
        var resultado = await Http.GetFromJsonAsync<List<FuncionarioDto>>("api/funcionarios");

        Funcionarios = resultado?
            .Select(x => new FuncionarioNotificacaoViewModel
            {
                Id = x.Id,
                NomeCompleto = x.NomeCompleto,
                Setor = x.Setor,
                Telefone = x.Telefone,
                Selecionado = false
            })
            .ToList() ?? new();

        PaginaAtual = 1;
    }

    protected async Task CarregarHistorico()
    {
        Historico = (await Http.GetFromJsonAsync<List<NotificacaoRequestDto>>("api/notificacoes")
                     ?? new())
            .Where(x => x.DataEnvio.HasValue &&
                        x.DataEnvio.Value >= DateTime.UtcNow.AddDays(-30))
            .OrderByDescending(x => x.DataEnvio)
            .ToList();

        PaginaAtualHistorico = 1;
    }

    protected void SelecionarTodos()
    {
        foreach (var funcionario in Funcionarios)
            funcionario.Selecionado = true;
    }

    protected void LimparSelecao()
    {
        foreach (var funcionario in Funcionarios)
            funcionario.Selecionado = false;
    }

    protected async Task EnviarNotificacao()
    {
        MensagemRetorno = null;

        var selecionados = Funcionarios.Where(x => x.Selecionado).ToList();

        if (!selecionados.Any())
        {
            MensagemRetorno = "Selecione pelo menos um funcionário.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Mensagem))
        {
            MensagemRetorno = "Digite uma mensagem antes de enviar.";
            return;
        }

        foreach (var funcionario in selecionados)
        {
            var dto = new NotificacaoRequestDto
            {
                FuncionarioId = funcionario.Id,
                Canal = "WhatsApp",
                Titulo = "Retirada de Cesta Básica",
                Mensagem = Mensagem,
                DataEnvio = DateTime.UtcNow
            };

            await Http.PostAsJsonAsync("api/notificacoes/enviar", dto);
        }

        MensagemRetorno = "Notificações enviadas com sucesso.";

        LimparSelecao();

        await CarregarHistorico();
    }

    protected void ProximaPagina()
    {
        if (PaginaAtual < TotalPaginas)
            PaginaAtual++;
    }

    protected void PaginaAnterior()
    {
        if (PaginaAtual > 1)
            PaginaAtual--;
    }

    protected class FuncionarioNotificacaoViewModel
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
        public string Setor { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public bool Selecionado { get; set; }
    }
}