using CestaBasica.Api.Services;
using CestaBasica.Shared.DTOs;

namespace CestaBasica.Api.Applications;

public class ImportacaoApplication
{
    private readonly ImportacaoService _service;

    public ImportacaoApplication(ImportacaoService service)
    {
        _service = service;
    }

    public async Task<ImportacaoResultadoDto> ImportarFuncionariosAsync(IFormFile arquivo)
    {
        return await _service.ImportarFuncionariosAsync(arquivo);
    }
}