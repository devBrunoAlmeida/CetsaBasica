using CestaBasica.Api.Models;
using CestaBasica.Api.Repositories;
using CestaBasica.Shared.DTOs;

namespace CestaBasica.Api.Services;

public class FuncionarioService
{
    private readonly FuncionarioRepository _repo;

    public FuncionarioService(FuncionarioRepository repo)
    {
        _repo = repo;
    }

    public async Task<Funcionario> CriarAsync(FuncionarioCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new Exception("Nome é obrigatório");

        var funcionario = new Funcionario
        {
            Nome = dto.Nome,
            Matricula = dto.Matricula,
            CodigoBarras = dto.CodigoBarras,
            Telefone = dto.Telefone,
            Setor = dto.Setor
        };

        return await _repo.CriarAsync(funcionario);
    }

    public async Task<List<Funcionario>> ListarAsync()
    {
        return await _repo.ListarAsync();
    }
}