using CestaBasica.Api.Models;
using CestaBasica.Api.Repositories;
using CestaBasica.Shared.DTOs;

namespace CestaBasica.Api.Services;

public class FuncionarioService
{
    private readonly FuncionarioRepository _funcionarioRepository;

    public FuncionarioService(FuncionarioRepository repo)
    {
        _funcionarioRepository = repo;
    }

    public async Task<Funcionario> CriarAsync(FuncionarioDto dto)
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

        return await _funcionarioRepository.CriarAsync(funcionario);
    }

    public async Task<List<Funcionario>> ListarAsync()
    {
        return await _funcionarioRepository.ListarAsync();
    }
    public async Task ExcluirAsync(int id)
    {
        await _funcionarioRepository.ExcluirAsync(id);
    }
    public async Task<Funcionario> AtualizarAsync(FuncionarioDto dto)
    {
        var funcionario =
            await _funcionarioRepository.BuscarPorIdAsync(dto.Id);

        if (funcionario is null)
            throw new Exception("Funcionário não encontrado.");

        funcionario.Nome = dto.Nome;
        funcionario.Telefone = dto.Telefone;
        funcionario.Matricula = dto.Matricula;
        funcionario.CodigoBarras = dto.CodigoBarras;
        funcionario.Setor = dto.Setor;

        return await _funcionarioRepository.AtualizarAsync(funcionario);
    }
}