using CestaBasica.Api.Models;
using CestaBasica.Api.Repositories;
using CestaBasica.Shared.DTOs;

namespace CestaBasica.Api.Services;

public class CestaService
{
    private readonly CestaRepository _repository;

    public CestaService(CestaRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Cesta>> ListarAsync()
    {
        return await _repository.ListarAsync();
    }

    public async Task CadastrarAsync(Cesta cesta)
    {
        if (string.IsNullOrWhiteSpace(cesta.Nome))
            throw new Exception("O nome da cesta é obrigatório.");

        if (cesta.QuantidadeDisponivel < 0)
            throw new Exception("A quantidade não pode ser negativa.");

        cesta.Ativa = true;
        cesta.DataCriacao = DateTime.UtcNow;

        await _repository.AdicionarAsync(cesta);
    }

    public async Task DesativarAsync(int id)
    {
        var cesta = await _repository.ObterPorIdAsync(id);

        if (cesta is null)
            throw new Exception("Cesta não encontrada.");

        if (!cesta.Ativa)
            throw new Exception("Cesta já está desativada.");

        cesta.Ativa = false;

        await _repository.AtualizarAsync(cesta);
    }
    public async Task AtualizarAsync(CestaDto dto)
    {
        var cesta = await _repository.ObterPorIdAsync(dto.Id);

        if (cesta == null)
            throw new Exception("Cesta não encontrada.");

        cesta.Nome = dto.Nome;
        cesta.Descricao = dto.Descricao;
        cesta.QuantidadeDisponivel = dto.QuantidadeDisponivel;

        await _repository.AtualizarAsync(cesta);
    }
    public async Task AtivarAsync(int id)
    {
        var cesta = await _repository.ObterPorIdAsync(id);

        if (cesta is null)
            throw new Exception("Cesta não encontrada.");

        if (cesta.Ativa)
            throw new Exception("Cesta já está ativa.");

        cesta.Ativa = true;

        await _repository.AtualizarAsync(cesta);
    }
}