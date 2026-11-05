using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CestaBasica.Api.Models;
using CestaBasica.Api.Repositories;
using CestaBasica.Shared.DTOs;

namespace CestaBasica.Api.Services;

public class EvolutionApiService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _configuration;
    private readonly ConfiguracoesRepository _configRepo;

    public EvolutionApiService(
        HttpClient http,
        IConfiguration configuration,
        ConfiguracoesRepository configRepo)
    {
        _http = http;
        _configuration = configuration;
        _configRepo = configRepo;
    }

    private async Task<(string BaseUrl, string ApiKey, string Instance, string Telefone)> ObterCredenciaisAsync()
    {
        var dbConfig = await _configRepo.ObterAsync();

        var baseUrl = !string.IsNullOrWhiteSpace(dbConfig?.EvolutionBaseUrl)
            ? dbConfig.EvolutionBaseUrl
            : (_configuration["EvolutionApi:BaseUrl"] ?? "http://localhost:8080");

        var apiKey = !string.IsNullOrWhiteSpace(dbConfig?.EvolutionApiKey)
            ? dbConfig.EvolutionApiKey
            : (_configuration["EvolutionApi:ApiKey"] ?? string.Empty);

        var instance = !string.IsNullOrWhiteSpace(dbConfig?.EvolutionInstance)
            ? dbConfig.EvolutionInstance
            : (_configuration["EvolutionApi:Instance"] ?? "cesta-basica");

        var telefone = dbConfig?.EvolutionTelefone ?? string.Empty;

        return (baseUrl.Trim().TrimEnd('/'), apiKey.Trim(), instance.Trim(), telefone.Trim());
    }

    private static string MascararChave(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            return string.Empty;
        if (apiKey.Length <= 6)
            return "••••••";
        return $"{apiKey[..3]}••••••{apiKey[^3..]}";
    }

    private static string NormalizarNumeroBrasil(string? telefone)
    {
        var digitos = new string((telefone ?? string.Empty).Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(digitos))
            return string.Empty;

        if (!digitos.StartsWith("55"))
            digitos = "55" + digitos;

        return digitos;
    }

    private static string FormatarJidTelefone(string? jid)
    {
        if (string.IsNullOrWhiteSpace(jid))
            return string.Empty;

        var numero = jid.Split('@')[0].Split(':')[0];
        if (numero.StartsWith("55") && (numero.Length == 12 || numero.Length == 13))
        {
            var ddd = numero.Substring(2, 2);
            var resto = numero.Substring(4);
            return resto.Length == 9
                ? $"+55 ({ddd}) {resto[..5]}-{resto[5..]}"
                : $"+55 ({ddd}) {resto[..4]}-{resto[4..]}";
        }

        return "+" + numero;
    }

    public async Task<WhatsappStatusDto> SalvarConfiguracaoAsync(WhatsappConfigDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.BaseUrl))
            throw new Exception("Informe a URL da Evolution API (ex: http://localhost:8080).");

        if (string.IsNullOrWhiteSpace(dto.InstanceName))
            throw new Exception("Informe o nome da instância (ex: cesta-basica).");

        var config = await _configRepo.ObterAsync();
        var criando = false;
        if (config is null)
        {
            config = new Configuracoes { WhatsAppHabilitado = true };
            criando = true;
        }

        var novaApiKey = dto.ApiKey?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(novaApiKey) && string.IsNullOrWhiteSpace(config.EvolutionApiKey))
            throw new Exception("Informe a API Key da Evolution API.");

        config.EvolutionBaseUrl = dto.BaseUrl.Trim().TrimEnd('/');
        if (!string.IsNullOrWhiteSpace(novaApiKey))
            config.EvolutionApiKey = novaApiKey;

        config.EvolutionInstance = dto.InstanceName.Trim();
        config.EvolutionTelefone = dto.Telefone?.Trim() ?? string.Empty;
        config.WhatsAppHabilitado = true;

        if (criando)
            await _configRepo.CriarAsync(config);
        else
            await _configRepo.AtualizarAsync(config);

        return await ObterStatusAsync();
    }

    public async Task<WhatsappStatusDto> ObterStatusAsync()
    {
        var (baseUrl, apiKey, instance, telefone) = await ObterCredenciaisAsync();

        var status = new WhatsappStatusDto
        {
            BaseUrl = baseUrl,
            InstanceName = instance,
            TelefoneConfigurado = telefone,
            ApiKeyMascarada = MascararChave(apiKey),
            Configurado = !string.IsNullOrWhiteSpace(baseUrl)
                          && !string.IsNullOrWhiteSpace(apiKey)
                          && !string.IsNullOrWhiteSpace(instance)
        };

        if (!status.Configurado)
        {
            status.Estado = "nao_configurado";
            status.Mensagem = "Configure a URL, a API Key e o nome da instância da Evolution API para conectar seu WhatsApp.";
            return status;
        }

        try
        {
            using var req = new HttpRequestMessage(
                HttpMethod.Get,
                $"{baseUrl}/instance/connectionState/{Uri.EscapeDataString(instance)}");
            req.Headers.Add("apikey", apiKey);

            var response = await _http.SendAsync(req);
            var content = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                status.Estado = "erro_autenticacao";
                status.Mensagem = "API Key inválida ou recusada pela Evolution API.";
                return status;
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                status.Estado = "close";
                status.Conectado = false;
                status.Mensagem = "Instância ainda não criada na Evolution API. Clique em 'Conectar meu WhatsApp' para criar e gerar o QR Code automaticamente.";
                return status;
            }

            if (!response.IsSuccessStatusCode)
            {
                status.Estado = "erro";
                status.Mensagem = $"Evolution API retornou {(int)response.StatusCode}: verifique a URL e a chave.";
                return status;
            }

            using var json = JsonDocument.Parse(content);
            var state = "close";
            if (json.RootElement.TryGetProperty("instance", out var instEl) &&
                instEl.TryGetProperty("state", out var stateEl))
            {
                state = stateEl.GetString() ?? "close";
            }
            else if (json.RootElement.TryGetProperty("state", out var rootState))
            {
                state = rootState.GetString() ?? "close";
            }

            status.Estado = state;
            status.Conectado = string.Equals(state, "open", StringComparison.OrdinalIgnoreCase);

            if (status.Conectado)
            {
                await PreencherDadosPerfilConectadoAsync(baseUrl, apiKey, instance, status);
                status.Mensagem = "Telefone conectado e pronto para enviar notificações.";
            }
            else if (string.Equals(state, "connecting", StringComparison.OrdinalIgnoreCase))
            {
                status.Mensagem = "Aguardando leitura do QR Code ou código de pareamento no celular.";
            }
            else
            {
                status.Mensagem = "Telefone desconectado. Gere o QR Code para vincular o aparelho.";
            }

            return status;
        }
        catch (Exception ex)
        {
            status.Estado = "erro_conexao";
            status.Conectado = false;
            status.Mensagem = $"Não foi possível alcançar a Evolution API em {baseUrl} ({ex.Message}).";
            return status;
        }
    }

    private async Task PreencherDadosPerfilConectadoAsync(
        string baseUrl,
        string apiKey,
        string instance,
        WhatsappStatusDto status)
    {
        try
        {
            using var req = new HttpRequestMessage(
                HttpMethod.Get,
                $"{baseUrl}/instance/fetchInstances?instanceName={Uri.EscapeDataString(instance)}");
            req.Headers.Add("apikey", apiKey);

            var resp = await _http.SendAsync(req);
            if (!resp.IsSuccessStatusCode)
                return;

            var body = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);

            JsonElement item = default;
            var found = false;

            if (doc.RootElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var el in doc.RootElement.EnumerateArray())
                {
                    var nome = el.TryGetProperty("name", out var nEl) ? nEl.GetString()
                        : (el.TryGetProperty("instance", out var sub) && sub.TryGetProperty("instanceName", out var inEl) ? inEl.GetString() : null);

                    if (string.Equals(nome, instance, StringComparison.OrdinalIgnoreCase))
                    {
                        item = el;
                        found = true;
                        break;
                    }
                }

                if (!found && doc.RootElement.GetArrayLength() > 0)
                {
                    item = doc.RootElement[0];
                    found = true;
                }
            }
            else if (doc.RootElement.ValueKind == JsonValueKind.Object)
            {
                item = doc.RootElement;
                found = true;
            }

            if (!found)
                return;

            string? ownerJid = null;
            string? profileName = null;
            string? profilePic = null;

            if (item.TryGetProperty("ownerJid", out var oj) && oj.ValueKind == JsonValueKind.String)
                ownerJid = oj.GetString();
            else if (item.TryGetProperty("instance", out var subInst) && subInst.TryGetProperty("owner", out var ow) && ow.ValueKind == JsonValueKind.String)
                ownerJid = ow.GetString();

            if (item.TryGetProperty("profileName", out var pn) && pn.ValueKind == JsonValueKind.String)
                profileName = pn.GetString();
            else if (item.TryGetProperty("instance", out var subInst2) && subInst2.TryGetProperty("profileName", out var pn2) && pn2.ValueKind == JsonValueKind.String)
                profileName = pn2.GetString();

            if (item.TryGetProperty("profilePicUrl", out var pp) && pp.ValueKind == JsonValueKind.String)
                profilePic = pp.GetString();

            status.NumeroConectado = FormatarJidTelefone(ownerJid);
            status.PerfilNome = profileName;
            status.FotoUrl = profilePic;
        }
        catch
        {
            // Opcional: falha ao enriquecer perfil não invalida o estado conectado
        }
    }

    public async Task<WhatsappConexaoResultadoDto> ConectarAsync(WhatsappConectarRequestDto dto)
    {
        var (baseUrl, apiKey, instance, _) = await ObterCredenciaisAsync();

        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(instance))
            throw new Exception("Configure primeiro a URL, a API Key e a Instância da Evolution API.");

        var numeroNormalizado = NormalizarNumeroBrasil(dto.Telefone);

        if (dto.UsarCodigoPareamento && string.IsNullOrWhiteSpace(numeroNormalizado))
            throw new Exception("Informe o número do WhatsApp com DDD para gerar o código de pareamento.");

        if (!string.IsNullOrWhiteSpace(dto.Telefone))
        {
            var config = await _configRepo.ObterAsync();
            if (config is not null)
            {
                config.EvolutionTelefone = dto.Telefone.Trim();
                await _configRepo.AtualizarAsync(config);
            }
        }

        // 1. Verificar se a instância já existe e qual o estado atual
        using var checkReq = new HttpRequestMessage(
            HttpMethod.Get,
            $"{baseUrl}/instance/connectionState/{Uri.EscapeDataString(instance)}");
        checkReq.Headers.Add("apikey", apiKey);

        var checkResp = await _http.SendAsync(checkReq);
        var checkBody = await checkResp.Content.ReadAsStringAsync();

        if (checkResp.StatusCode == HttpStatusCode.Unauthorized || checkResp.StatusCode == HttpStatusCode.Forbidden)
            throw new Exception("API Key recusada pela Evolution API. Verifique a chave informada.");

        string? qrBase64 = null;
        string? pairingCode = null;

        if (checkResp.StatusCode == HttpStatusCode.NotFound)
        {
            var createContent = await CriarInstanciaEvolutionAsync(
                baseUrl,
                apiKey,
                instance,
                dto.UsarCodigoPareamento ? numeroNormalizado : null);

            ExtrairQrOuPairing(createContent, out qrBase64, out pairingCode);
        }
        else if (checkResp.IsSuccessStatusCode)
        {
            using var stateDoc = JsonDocument.Parse(checkBody);
            var currentState = stateDoc.RootElement.TryGetProperty("instance", out var instEl)
                && instEl.TryGetProperty("state", out var stEl)
                ? stEl.GetString()
                : null;

            if (string.Equals(currentState, "open", StringComparison.OrdinalIgnoreCase))
            {
                return new WhatsappConexaoResultadoDto
                {
                    Conectado = true,
                    Estado = "open",
                    Mensagem = "Este aparelho já está conectado ao WhatsApp!"
                };
            }
        }

        // 2. Caso o QR Code ou Pairing Code ainda não tenha vindo, chamar /instance/connect/{instance}
        var connectUrl = $"{baseUrl}/instance/connect/{Uri.EscapeDataString(instance)}";
        if (dto.UsarCodigoPareamento && !string.IsNullOrWhiteSpace(numeroNormalizado))
            connectUrl += $"?number={numeroNormalizado}";

        if (string.IsNullOrWhiteSpace(qrBase64) && string.IsNullOrWhiteSpace(pairingCode))
        {
            using var connectReq = new HttpRequestMessage(HttpMethod.Get, connectUrl);
            connectReq.Headers.Add("apikey", apiKey);

            var connectResp = await _http.SendAsync(connectReq);
            var connectContent = await connectResp.Content.ReadAsStringAsync();

            if (connectResp.IsSuccessStatusCode)
                ExtrairQrOuPairing(connectContent, out qrBase64, out pairingCode);

            // Se a instância anterior ficou presa (count: 0 / close / chaves antigas), recria limpa e gera novo QR
            if (string.IsNullOrWhiteSpace(qrBase64) && string.IsNullOrWhiteSpace(pairingCode))
            {
                await RemoverInstanciaSilenciosoAsync(baseUrl, apiKey, instance);
                await Task.Delay(500);

                var recreateContent = await CriarInstanciaEvolutionAsync(
                    baseUrl,
                    apiKey,
                    instance,
                    dto.UsarCodigoPareamento ? numeroNormalizado : null);

                ExtrairQrOuPairing(recreateContent, out qrBase64, out pairingCode);

                if (string.IsNullOrWhiteSpace(qrBase64) && string.IsNullOrWhiteSpace(pairingCode))
                {
                    using var retryReq = new HttpRequestMessage(HttpMethod.Get, connectUrl);
                    retryReq.Headers.Add("apikey", apiKey);
                    var retryResp = await _http.SendAsync(retryReq);
                    var retryContent = await retryResp.Content.ReadAsStringAsync();
                    if (retryResp.IsSuccessStatusCode)
                        ExtrairQrOuPairing(retryContent, out qrBase64, out pairingCode);
                }
            }
        }

        return new WhatsappConexaoResultadoDto
        {
            Conectado = false,
            Estado = "connecting",
            QrCodeBase64 = qrBase64,
            PairingCode = pairingCode,
            Mensagem = !string.IsNullOrWhiteSpace(pairingCode)
                ? "Digite o código de pareamento abaixo na notificação do WhatsApp em seu celular."
                : "Abra o WhatsApp no seu celular → Aparelhos conectados → Conectar um aparelho e escaneie o QR Code."
        };
    }

    private async Task<string> CriarInstanciaEvolutionAsync(
        string baseUrl,
        string apiKey,
        string instance,
        string? numeroPareamento)
    {
        // Nota Evolution API v2: só enviar 'number' na criação quando o usuário optar por Código de Pareamento;
        // caso contrário a instância entra em loop de pairingCode e invalida o QR Code rapidamente.
        object createPayload = !string.IsNullOrWhiteSpace(numeroPareamento)
            ? new
            {
                instanceName = instance,
                qrcode = true,
                number = numeroPareamento,
                integration = "WHATSAPP-BAILEYS",
                groupsIgnore = true,
                syncFullHistory = false
            }
            : new
            {
                instanceName = instance,
                qrcode = true,
                integration = "WHATSAPP-BAILEYS",
                groupsIgnore = true,
                syncFullHistory = false
            };

        using var createReq = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/instance/create");
        createReq.Headers.Add("apikey", apiKey);
        createReq.Content = JsonContent.Create(createPayload);

        var createResp = await _http.SendAsync(createReq);
        var createContent = await createResp.Content.ReadAsStringAsync();

        if (!createResp.IsSuccessStatusCode)
            throw new Exception($"Erro ao criar instância '{instance}' na Evolution API: {createContent}");

        return createContent;
    }

    private static void ExtrairQrOuPairing(string jsonContent, out string? qrBase64, out string? pairingCode)
    {
        qrBase64 = null;
        pairingCode = null;

        if (string.IsNullOrWhiteSpace(jsonContent))
            return;

        try
        {
            using var doc = JsonDocument.Parse(jsonContent);
            var root = doc.RootElement;

            if (root.TryGetProperty("base64", out var b64El) && b64El.ValueKind == JsonValueKind.String)
                qrBase64 = b64El.GetString();

            if (root.TryGetProperty("pairingCode", out var pcEl) && pcEl.ValueKind == JsonValueKind.String)
                pairingCode = pcEl.GetString();

            if (root.TryGetProperty("qrcode", out var qrObj) && qrObj.ValueKind == JsonValueKind.Object)
            {
                if (string.IsNullOrWhiteSpace(qrBase64) &&
                    qrObj.TryGetProperty("base64", out var nestedB64) &&
                    nestedB64.ValueKind == JsonValueKind.String)
                {
                    qrBase64 = nestedB64.GetString();
                }

                if (string.IsNullOrWhiteSpace(pairingCode) &&
                    qrObj.TryGetProperty("pairingCode", out var nestedPc) &&
                    nestedPc.ValueKind == JsonValueKind.String)
                {
                    pairingCode = nestedPc.GetString();
                }
            }

            if (!string.IsNullOrWhiteSpace(qrBase64) && !qrBase64.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
                qrBase64 = $"data:image/png;base64,{qrBase64}";
        }
        catch
        {
            // Ignora JSON inesperado
        }
    }

    private async Task RemoverInstanciaSilenciosoAsync(string baseUrl, string apiKey, string instance)
    {
        try
        {
            using var logoutReq = new HttpRequestMessage(
                HttpMethod.Delete,
                $"{baseUrl}/instance/logout/{Uri.EscapeDataString(instance)}");
            logoutReq.Headers.Add("apikey", apiKey);
            await _http.SendAsync(logoutReq);
        }
        catch
        {
            // Ignora se já desconectada
        }

        try
        {
            using var delReq = new HttpRequestMessage(
                HttpMethod.Delete,
                $"{baseUrl}/instance/delete/{Uri.EscapeDataString(instance)}");
            delReq.Headers.Add("apikey", apiKey);
            await _http.SendAsync(delReq);
        }
        catch
        {
            // Ignora
        }
    }

    public async Task<WhatsappStatusDto> DesconectarAsync()
    {
        var (baseUrl, apiKey, instance, _) = await ObterCredenciaisAsync();

        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(instance))
            throw new Exception("Evolution API não configurada.");

        using var req = new HttpRequestMessage(
            HttpMethod.Delete,
            $"{baseUrl}/instance/logout/{Uri.EscapeDataString(instance)}");
        req.Headers.Add("apikey", apiKey);

        var resp = await _http.SendAsync(req);
        if (!resp.IsSuccessStatusCode && resp.StatusCode != HttpStatusCode.NotFound && resp.StatusCode != HttpStatusCode.BadRequest)
        {
            var erro = await resp.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao desconectar aparelho: {erro}");
        }

        // Remove também a instância desconectada para limpar prekeys/sessão e permitir conectar outro aparelho sem conflito
        try
        {
            using var delReq = new HttpRequestMessage(
                HttpMethod.Delete,
                $"{baseUrl}/instance/delete/{Uri.EscapeDataString(instance)}");
            delReq.Headers.Add("apikey", apiKey);
            await _http.SendAsync(delReq);
        }
        catch
        {
            // Ignora
        }

        return await ObterStatusAsync();
    }

    public async Task<string> EnviarTextoAsync(string telefone, string mensagem)
    {
        var (baseUrl, apiKey, instance, _) = await ObterCredenciaisAsync();

        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new Exception("URL da Evolution API não configurada.");

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new Exception("API Key da Evolution API não configurada.");

        if (string.IsNullOrWhiteSpace(instance))
            throw new Exception("Instância da Evolution API não configurada.");

        var numero = NormalizarNumeroBrasil(telefone);
        if (string.IsNullOrWhiteSpace(numero))
            throw new Exception("Número de telefone inválido.");

        var body = new
        {
            number = numero,
            text = mensagem
        };

        var url = $"{baseUrl}/message/sendText/{Uri.EscapeDataString(instance)}";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("apikey", apiKey);
        request.Content = JsonContent.Create(body);

        var response = await _http.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Falha Evolution API ({(int)response.StatusCode}): {content}");

        try
        {
            using var json = JsonDocument.Parse(content);
            if (json.RootElement.TryGetProperty("key", out var keyEl) &&
                keyEl.TryGetProperty("id", out var idEl))
            {
                return idEl.GetString() ?? content;
            }
        }
        catch
        {
            // Retorna conteúdo bruto caso formato mude
        }

        return content;
    }
}
