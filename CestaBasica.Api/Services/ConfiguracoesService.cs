using CestaBasica.Api.Models;
using CestaBasica.Api.Repositories;
using CestaBasica.Shared.DTOs;

namespace CestaBasica.Api.Services;

public class ConfiguracoesService
{
    private readonly ConfiguracoesRepository _repository;
    private readonly IConfiguration _configuration;

    public ConfiguracoesService(ConfiguracoesRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<ConfiguracoesDto> ObterAsync()
    {
        var config = await _repository.ObterAsync();

        if (config == null)
        {
            config = new Configuracoes
            {
                EvolutionBaseUrl = _configuration["EvolutionApi:BaseUrl"] ?? "http://localhost:8080",
                EvolutionApiKey = _configuration["EvolutionApi:ApiKey"] ?? string.Empty,
                EvolutionInstance = _configuration["EvolutionApi:Instance"] ?? "cesta-basica"
            };

            await _repository.CriarAsync(config);
        }

        return new ConfiguracoesDto
        {
            WhatsAppHabilitado = config.WhatsAppHabilitado,
            SmsHabilitado = config.SmsHabilitado,
            EnvioAutomaticoLembretes = config.EnvioAutomaticoLembretes,
            TempoSessaoMinutos = config.TempoSessaoMinutos,
            EncerrarPorInatividade = config.EncerrarPorInatividade,
            FrequenciaBackup = config.FrequenciaBackup,
            PermitirCadastroOperadores = config.PermitirCadastroOperadores,
            BloqueioTentativasInvalidas = config.BloqueioTentativasInvalidas,
            EvolutionBaseUrl = string.IsNullOrWhiteSpace(config.EvolutionBaseUrl)
                ? (_configuration["EvolutionApi:BaseUrl"] ?? "http://localhost:8080")
                : config.EvolutionBaseUrl,
            EvolutionApiKey = string.IsNullOrWhiteSpace(config.EvolutionApiKey)
                ? (_configuration["EvolutionApi:ApiKey"] ?? string.Empty)
                : config.EvolutionApiKey,
            EvolutionInstance = string.IsNullOrWhiteSpace(config.EvolutionInstance)
                ? (_configuration["EvolutionApi:Instance"] ?? "cesta-basica")
                : config.EvolutionInstance,
            EvolutionTelefone = config.EvolutionTelefone
        };
    }

    public async Task<ConfiguracoesDto> AtualizarAsync(ConfiguracoesDto dto)
    {
        var config = await _repository.ObterAsync();

        if (config == null)
        {
            config = new Configuracoes();
            await _repository.CriarAsync(config);
        }

        config.WhatsAppHabilitado = dto.WhatsAppHabilitado;
        config.SmsHabilitado = dto.SmsHabilitado;
        config.EnvioAutomaticoLembretes = dto.EnvioAutomaticoLembretes;
        config.TempoSessaoMinutos = dto.TempoSessaoMinutos;
        config.EncerrarPorInatividade = dto.EncerrarPorInatividade;
        config.FrequenciaBackup = dto.FrequenciaBackup;
        config.PermitirCadastroOperadores = dto.PermitirCadastroOperadores;
        config.BloqueioTentativasInvalidas = dto.BloqueioTentativasInvalidas;
        config.EvolutionBaseUrl = dto.EvolutionBaseUrl?.Trim().TrimEnd('/') ?? "http://localhost:8080";
        if (!string.IsNullOrWhiteSpace(dto.EvolutionApiKey))
            config.EvolutionApiKey = dto.EvolutionApiKey.Trim();
        config.EvolutionInstance = string.IsNullOrWhiteSpace(dto.EvolutionInstance)
            ? "cesta-basica"
            : dto.EvolutionInstance.Trim();
        config.EvolutionTelefone = dto.EvolutionTelefone?.Trim() ?? string.Empty;

        await _repository.AtualizarAsync(config);

        return await ObterAsync();
    }
}
