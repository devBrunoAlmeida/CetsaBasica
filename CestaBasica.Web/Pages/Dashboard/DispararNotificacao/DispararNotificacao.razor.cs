using CestaBasica.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Text.Json;

namespace CestaBasica.Web.Pages.Dashboard.DispararNotificacao;

public partial class DispararNotificacao : ComponentBase, IDisposable
{
    [Inject] private HttpClient Http { get; set; } = default!;

    protected string Mensagem { get; set; } =
        "Prezado(a), informamos que sua cesta básica está disponível para retirada no setor responsável.";

    protected string? MensagemRetorno { get; set; }
    protected bool RetornoErro { get; set; }
    protected bool EnviandoNotificacoes { get; set; }

    protected string TermoBuscaFuncionario { get; set; } = string.Empty;
    protected List<FuncionarioNotificacaoViewModel> Funcionarios { get; set; } = new();
    protected List<NotificacaoRequestDto> Historico { get; set; } = new();

    protected int PaginaAtual { get; set; } = 1;
    protected int ItensPorPagina { get; set; } = 10;
    protected int PaginaAtualHistorico = 1;
    protected int ItensPorPaginaHistorico = 10;

    // Estado de conexão WhatsApp / Evolution API
    protected WhatsappStatusDto StatusWhatsapp { get; set; } = new();
    protected WhatsappConfigDto FormConfig { get; set; } = new();
    protected bool CarregandoWhatsapp { get; set; }
    protected bool ExibirPainelConfig { get; set; }
    protected bool ExibirPainelPareamento { get; set; }
    protected bool ExibirPainelTeste { get; set; }
    protected bool UsarCodigoPareamento { get; set; }
    protected string TelefonePareamento { get; set; } = string.Empty;
    protected string TelefoneTeste { get; set; } = string.Empty;
    protected string? QrCodeBase64 { get; set; }
    protected string? PairingCode { get; set; }
    protected string? FeedbackWhatsapp { get; set; }
    protected bool FeedbackWhatsappErro { get; set; }

    private CancellationTokenSource? _pollingCts;

    protected IEnumerable<FuncionarioNotificacaoViewModel> FuncionariosFiltrados =>
        string.IsNullOrWhiteSpace(TermoBuscaFuncionario)
            ? Funcionarios
            : Funcionarios.Where(f =>
                f.NomeCompleto.Contains(TermoBuscaFuncionario, StringComparison.OrdinalIgnoreCase) ||
                f.Setor.Contains(TermoBuscaFuncionario, StringComparison.OrdinalIgnoreCase) ||
                f.Telefone.Contains(TermoBuscaFuncionario, StringComparison.OrdinalIgnoreCase));

    protected int QuantidadeSelecionados => Funcionarios.Count(x => x.Selecionado);

    protected int TotalPaginas =>
        !FuncionariosFiltrados.Any()
            ? 1
            : (int)Math.Ceiling((double)FuncionariosFiltrados.Count() / ItensPorPagina);

    protected List<FuncionarioNotificacaoViewModel> FuncionariosPaginados =>
        FuncionariosFiltrados
            .Skip((PaginaAtual - 1) * ItensPorPagina)
            .Take(ItensPorPagina)
            .ToList();

    protected int TotalPaginasHistorico =>
        Historico.Count == 0
            ? 1
            : (int)Math.Ceiling((double)Historico.Count / ItensPorPaginaHistorico);

    protected IEnumerable<NotificacaoRequestDto> HistoricoPaginado =>
        Historico
            .OrderByDescending(x => x.DataEnvio)
            .Skip((PaginaAtualHistorico - 1) * ItensPorPaginaHistorico)
            .Take(ItensPorPaginaHistorico);

    protected override async Task OnInitializedAsync()
    {
        await Task.WhenAll(
            CarregarStatusWhatsappAsync(),
            CarregarFuncionarios(),
            CarregarHistorico()
        );
    }

    protected async Task CarregarStatusWhatsappAsync()
    {
        try
        {
            var status = await Http.GetFromJsonAsync<WhatsappStatusDto>("api/notificacoes/whatsapp/status");
            if (status is not null)
            {
                StatusWhatsapp = status;
                FormConfig.BaseUrl = string.IsNullOrWhiteSpace(status.BaseUrl) ? "http://localhost:8080" : status.BaseUrl;
                FormConfig.InstanceName = string.IsNullOrWhiteSpace(status.InstanceName) ? "cesta-basica" : status.InstanceName;
                FormConfig.Telefone = status.TelefoneConfigurado;

                if (string.IsNullOrWhiteSpace(TelefonePareamento) && !string.IsNullOrWhiteSpace(status.TelefoneConfigurado))
                    TelefonePareamento = status.TelefoneConfigurado;

                if (status.Conectado)
                {
                    PararMonitoramentoConexao();
                    ExibirPainelPareamento = false;
                    QrCodeBase64 = null;
                    PairingCode = null;
                }
            }
        }
        catch (Exception ex)
        {
            FeedbackWhatsappErro = true;
            FeedbackWhatsapp = $"Erro ao verificar status do WhatsApp: {ex.Message}";
        }
    }

    protected void AlternarPainelConfig()
    {
        FeedbackWhatsapp = null;
        ExibirPainelConfig = !ExibirPainelConfig;
        if (ExibirPainelConfig)
            ExibirPainelTeste = false;
    }

    protected void AlternarPainelTeste()
    {
        FeedbackWhatsapp = null;
        ExibirPainelTeste = !ExibirPainelTeste;
        if (ExibirPainelTeste)
            ExibirPainelConfig = false;
    }

    protected async Task SalvarConfiguracaoWhatsappAsync(bool conectarEmSeguida = false)
    {
        CarregandoWhatsapp = true;
        FeedbackWhatsapp = null;
        FeedbackWhatsappErro = false;

        try
        {
            var response = await Http.PutAsJsonAsync("api/notificacoes/whatsapp/configurar", FormConfig);
            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadFromJsonAsync<ErroApiDto>();
                FeedbackWhatsappErro = true;
                FeedbackWhatsapp = erro?.Mensagem ?? "Erro ao salvar configuração da Evolution API.";
                return;
            }

            var novoStatus = await response.Content.ReadFromJsonAsync<WhatsappStatusDto>();
            if (novoStatus is not null)
                StatusWhatsapp = novoStatus;

            FormConfig.ApiKey = string.Empty;
            ExibirPainelConfig = false;
            FeedbackWhatsapp = "Configuração da Evolution API salva com sucesso!";

            if (conectarEmSeguida && !StatusWhatsapp.Conectado)
            {
                await AbrirEConectarTelefoneAsync();
            }
        }
        catch (Exception ex)
        {
            FeedbackWhatsappErro = true;
            FeedbackWhatsapp = $"Erro ao salvar configuração: {ex.Message}";
        }
        finally
        {
            CarregandoWhatsapp = false;
        }
    }

    protected async Task AbrirEConectarTelefoneAsync()
    {
        if (!StatusWhatsapp.Configurado)
        {
            ExibirPainelConfig = true;
            FeedbackWhatsappErro = true;
            FeedbackWhatsapp = "Preencha primeiro a URL, a API Key e o Nome da Instância da Evolution API.";
            return;
        }

        ExibirPainelConfig = false;
        ExibirPainelPareamento = true;
        await GerarConexaoTelefoneAsync();
    }

    protected async Task GerarConexaoTelefoneAsync()
    {
        CarregandoWhatsapp = true;
        FeedbackWhatsapp = null;
        FeedbackWhatsappErro = false;
        QrCodeBase64 = null;
        PairingCode = null;
        StateHasChanged();

        try
        {
            var req = new WhatsappConectarRequestDto
            {
                Telefone = TelefonePareamento,
                UsarCodigoPareamento = UsarCodigoPareamento
            };

            var response = await Http.PostAsJsonAsync("api/notificacoes/whatsapp/conectar", req);
            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadFromJsonAsync<ErroApiDto>();
                FeedbackWhatsappErro = true;
                FeedbackWhatsapp = erro?.Mensagem ?? "Não foi possível gerar o QR Code na Evolution API.";
                return;
            }

            var resultado = await response.Content.ReadFromJsonAsync<WhatsappConexaoResultadoDto>();
            if (resultado is null)
                return;

            if (resultado.Conectado)
            {
                await CarregarStatusWhatsappAsync();
                FeedbackWhatsapp = "✅ Seu telefone já está conectado!";
                return;
            }

            QrCodeBase64 = resultado.QrCodeBase64;
            PairingCode = resultado.PairingCode;
            FeedbackWhatsapp = resultado.Mensagem;

            if (string.IsNullOrWhiteSpace(QrCodeBase64) && string.IsNullOrWhiteSpace(PairingCode))
            {
                FeedbackWhatsappErro = true;
                FeedbackWhatsapp = "A Evolution API não retornou QR Code ainda. Aguarde alguns segundos e clique em 'Atualizar QR Code'.";
            }
            else
            {
                IniciarMonitoramentoConexao();
            }
        }
        catch (Exception ex)
        {
            FeedbackWhatsappErro = true;
            FeedbackWhatsapp = $"Falha de conexão com a Evolution API: {ex.Message}";
        }
        finally
        {
            CarregandoWhatsapp = false;
        }
    }

    private void IniciarMonitoramentoConexao()
    {
        PararMonitoramentoConexao();
        _pollingCts = new CancellationTokenSource();
        _ = MonitorarConexaoAsync(_pollingCts.Token);
    }

    private async Task MonitorarConexaoAsync(CancellationToken token)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(3));
        var tentativas = 0;

        try
        {
            while (tentativas < 60 && await timer.WaitForNextTickAsync(token))
            {
                tentativas++;
                var status = await Http.GetFromJsonAsync<WhatsappStatusDto>("api/notificacoes/whatsapp/status", token);
                if (status is not null && status.Conectado)
                {
                    StatusWhatsapp = status;
                    ExibirPainelPareamento = false;
                    QrCodeBase64 = null;
                    PairingCode = null;
                    FeedbackWhatsappErro = false;
                    FeedbackWhatsapp = "✅ Telefone vinculado com sucesso! Agora você já pode disparar notificações.";
                    StateHasChanged();
                    break;
                }

                // Renova automaticamente o QR Code antes de expirar (~36s) enquanto aguarda leitura
                if (tentativas % 12 == 0 &&
                    ExibirPainelPareamento &&
                    !UsarCodigoPareamento &&
                    !CarregandoWhatsapp)
                {
                    var req = new WhatsappConectarRequestDto
                    {
                        Telefone = TelefonePareamento,
                        UsarCodigoPareamento = false
                    };
                    var resp = await Http.PostAsJsonAsync("api/notificacoes/whatsapp/conectar", req, token);
                    if (resp.IsSuccessStatusCode)
                    {
                        var novoQr = await resp.Content.ReadFromJsonAsync<WhatsappConexaoResultadoDto>(cancellationToken: token);
                        if (novoQr is not null && !string.IsNullOrWhiteSpace(novoQr.QrCodeBase64))
                        {
                            QrCodeBase64 = novoQr.QrCodeBase64;
                            StateHasChanged();
                        }
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Monitoramento encerrado normalmente
        }
        catch
        {
            // Ignora falhas transitórias de rede durante polling
        }
    }

    private void PararMonitoramentoConexao()
    {
        if (_pollingCts is not null)
        {
            _pollingCts.Cancel();
            _pollingCts.Dispose();
            _pollingCts = null;
        }
    }

    protected void FecharPainelPareamento()
    {
        PararMonitoramentoConexao();
        ExibirPainelPareamento = false;
        QrCodeBase64 = null;
        PairingCode = null;
    }

    protected async Task DesconectarTelefoneAsync()
    {
        CarregandoWhatsapp = true;
        FeedbackWhatsapp = null;
        FeedbackWhatsappErro = false;
        PararMonitoramentoConexao();

        try
        {
            var response = await Http.PostAsync("api/notificacoes/whatsapp/desconectar", null);
            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadFromJsonAsync<ErroApiDto>();
                FeedbackWhatsappErro = true;
                FeedbackWhatsapp = erro?.Mensagem ?? "Erro ao desconectar o telefone.";
                return;
            }

            await CarregarStatusWhatsappAsync();
            FeedbackWhatsapp = "Telefone desconectado com sucesso. Você pode conectar outro aparelho quando desejar.";
        }
        catch (Exception ex)
        {
            FeedbackWhatsappErro = true;
            FeedbackWhatsapp = $"Erro ao desconectar: {ex.Message}";
        }
        finally
        {
            CarregandoWhatsapp = false;
        }
    }

    protected async Task EnviarMensagemTesteAsync()
    {
        if (string.IsNullOrWhiteSpace(TelefoneTeste))
        {
            FeedbackWhatsappErro = true;
            FeedbackWhatsapp = "Digite um número com DDD para enviar o teste.";
            return;
        }

        CarregandoWhatsapp = true;
        FeedbackWhatsapp = null;
        FeedbackWhatsappErro = false;

        try
        {
            var response = await Http.PostAsJsonAsync("api/notificacoes/whatsapp/teste", new WhatsappTesteEnvioDto
            {
                Telefone = TelefoneTeste
            });

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadFromJsonAsync<ErroApiDto>();
                FeedbackWhatsappErro = true;
                FeedbackWhatsapp = erro?.Mensagem ?? "Falha ao enviar mensagem de teste.";
                return;
            }

            FeedbackWhatsapp = $"✅ Mensagem de teste enviada com sucesso para {TelefoneTeste}!";
            ExibirPainelTeste = false;
        }
        catch (Exception ex)
        {
            FeedbackWhatsappErro = true;
            FeedbackWhatsapp = $"Erro no envio de teste: {ex.Message}";
        }
        finally
        {
            CarregandoWhatsapp = false;
        }
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

    protected void AoBuscarFuncionario()
    {
        PaginaAtual = 1;
    }

    protected void SelecionarTodosFiltrados()
    {
        foreach (var funcionario in FuncionariosFiltrados)
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
        RetornoErro = false;

        var selecionados = Funcionarios.Where(x => x.Selecionado).ToList();

        if (!selecionados.Any())
        {
            RetornoErro = true;
            MensagemRetorno = "Selecione pelo menos um funcionário.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Mensagem))
        {
            RetornoErro = true;
            MensagemRetorno = "Digite uma mensagem antes de enviar.";
            return;
        }

        if (!StatusWhatsapp.Conectado)
        {
            RetornoErro = true;
            MensagemRetorno = "Nenhum telefone WhatsApp está conectado no momento. Conecte seu aparelho no card acima antes de disparar.";
            return;
        }

        EnviandoNotificacoes = true;
        var enviados = 0;
        var falhas = 0;

        try
        {
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

                var resp = await Http.PostAsJsonAsync("api/notificacoes/enviar", dto);
                if (resp.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
                    var st = doc.RootElement.TryGetProperty("status", out var stEl) ? stEl.GetString() : null;
                    if (string.Equals(st, "Enviado", StringComparison.OrdinalIgnoreCase))
                        enviados++;
                    else
                        falhas++;
                }
                else
                {
                    falhas++;
                }
            }

            if (falhas == 0)
            {
                RetornoErro = false;
                MensagemRetorno = $"✅ {enviados} notificação(ões) enviada(s) com sucesso!";
            }
            else
            {
                RetornoErro = enviados == 0;
                MensagemRetorno = $"Envio concluído: {enviados} enviada(s) com sucesso e {falhas} falha(s).";
            }

            LimparSelecao();
            await CarregarHistorico();
        }
        finally
        {
            EnviandoNotificacoes = false;
        }
    }

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

    public void Dispose()
    {
        PararMonitoramentoConexao();
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
