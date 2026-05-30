using CestaBasica.Api.Services;
using CestaBasica.Shared.DTOs;

namespace CestaBasica.Api.Applications;

public class ImportacaoApplication
{
    private readonly ImportacaoService _service;

    public ImportacaoApplication(ImportacaoService service)
        => _service = service;

    public Task<ImportacaoResultadoDto> ImportarFuncionariosAsync(IFormFile arquivo)
    {
        var stream = arquivo.OpenReadStream();
        return _service.ImportarAsync(stream, arquivo.FileName);
    }

    public async Task<List<HistoricoImportacaoDto>> ObterHistoricoAsync()
{
    return await _service.ObterHistoricoAsync();
}
    
}