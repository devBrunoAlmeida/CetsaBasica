using CestaBasica.Api.Models;
using CestaBasica.Api.Repositories;
using CestaBasica.Shared.DTOs;

namespace CestaBasica.Api.Services;

public class CestaService
{
    private readonly CestaRepository _repo;

    public CestaService(CestaRepository repo)
    {
        _repo = repo;
    }

    public async Task<Cesta> CriarAsync(CestaDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new Exception("Nome da cesta é obrigatório.");

        if (dto.QuantidadeDisponivel < 0)
            throw new Exception("Quantidade não pode ser negativa.");

        var cesta = new Cesta
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            QuantidadeDisponivel = dto.QuantidadeDisponivel
        };

        return await _repo.CriarAsync(cesta);
    }

    public async Task<List<Cesta>> ListarAsync()
    {
        return await _repo.ListarAsync();
    }
}