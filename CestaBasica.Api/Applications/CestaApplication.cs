using CestaBasica.Api.Models;
using CestaBasica.Api.Services;
using CestaBasica.Shared.DTOs;

namespace CestaBasica.Api.Applications;

public class CestaApplication
{
    private readonly CestaService _service;

    public CestaApplication(CestaService service)
    {
        _service = service;
    }

    public async Task<List<CestaDto>> ListarAsync()
    {
        var cestas = await _service.ListarAsync();

        return cestas.Select(c => new CestaDto
        {
            Id = c.Id,
            Nome = c.Nome,
            Descricao = c.Descricao,
            Ativa = c.Ativa,
            QuantidadeDisponivel = c.QuantidadeDisponivel,
            DataCriacao = c.DataCriacao
        }).ToList();
    }

    public async Task CadastrarAsync(CestaDto dto)
    {
        var cesta = new Cesta
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            QuantidadeDisponivel = dto.QuantidadeDisponivel
        };

        await _service.CadastrarAsync(cesta);
    }

    public async Task DesativarAsync(int id)
    {
        await _service.DesativarAsync(id);
    }
    public async Task AtualizarAsync(CestaDto dto)
    {
        await _service.AtualizarAsync(dto);
    }
    public async Task AtivarAsync(int id)
    {
        await _service.AtivarAsync(id);
    }
}