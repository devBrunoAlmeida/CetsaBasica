using CestaBasica.Api.Models;
using CestaBasica.Api.Repositories;
using CestaBasica.Shared.DTOs;

namespace CestaBasica.Api.Services;

public class ConfiguracoesService
{
    private readonly ConfiguracoesRepository _repository;

    public ConfiguracoesService(ConfiguracoesRepository repository)
    {
        _repository = repository;
    }

    public async Task<ConfiguracoesDto> ObterAsync()
    {
        var config = await _repository.ObterAsync();

        if (config == null)
        {
            config = new Configuracoes();

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
            BloqueioTentativasInvalidas = config.BloqueioTentativasInvalidas
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

        await _repository.AtualizarAsync(config);

        return dto;
    }
}