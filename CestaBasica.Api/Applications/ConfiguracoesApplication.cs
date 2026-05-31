using CestaBasica.Api.Services;
using CestaBasica.Shared.DTOs;

namespace CestaBasica.Api.Applications;

public class ConfiguracoesApplication
{
    private readonly ConfiguracoesService _service;

    public ConfiguracoesApplication(ConfiguracoesService service)
    {
        _service = service;
    }

    public Task<ConfiguracoesDto> ObterAsync()
        => _service.ObterAsync();

    public Task<ConfiguracoesDto> AtualizarAsync(ConfiguracoesDto dto)
        => _service.AtualizarAsync(dto);
}