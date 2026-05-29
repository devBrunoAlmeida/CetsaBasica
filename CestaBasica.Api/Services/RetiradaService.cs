using CestaBasica.Api.Models;
using CestaBasica.Api.Repositories;
using CestaBasica.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CestaBasica.Api.Services;

public class RetiradaService
{
    private readonly FuncionarioRepository _funcionarioRepository;
    private readonly CestaRepository _cestaRepository;
    private readonly RetiradaRepository _retiradaRepository;

    public RetiradaService(
        FuncionarioRepository funcionarioRepository,
        CestaRepository cestaRepository,
        RetiradaRepository retiradaRepository)
    {
        _funcionarioRepository = funcionarioRepository;
        _cestaRepository = cestaRepository;
        _retiradaRepository = retiradaRepository;
    }

    public async Task<Retirada> RegistrarRetiradaAsync(RetiradaCestaDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CodigoBarras))
            throw new Exception("Código de barras é obrigatório.");

        var funcionario = await _funcionarioRepository.BuscarPorCodigoBarrasAsync(dto.CodigoBarras);

        if (funcionario == null)
            throw new Exception("Funcionário não encontrado.");

        var cesta = await _cestaRepository.ObterPorIdAsync(dto.Id);

        if (cesta == null)
            throw new Exception("Cesta não encontrada.");

        if (!cesta.Ativa)
            throw new Exception("Cesta inativa.");

        if (cesta.QuantidadeDisponivel <= 0)
            throw new Exception("Não há cestas disponíveis em estoque.");

        var jaRetirou = await _retiradaRepository.JaRetirouAsync(funcionario.Id, cesta.Id);

        if (jaRetirou)
            throw new Exception("Este funcionário já retirou esta cesta.");

        var retirada = new Retirada
        {
            FuncionarioId = funcionario.Id,
            CestaId = cesta.Id,
            DataRetirada = DateTime.UtcNow
        };

        cesta.QuantidadeDisponivel--;

        await _cestaRepository.AtualizarAsync(cesta);
        return await _retiradaRepository.CriarAsync(retirada);
    }

    public async Task<List<Retirada>> ListarAsync()
    {
        return await _retiradaRepository.ListarAsync();
    }
    public async Task<List<FuncionarioDto>> ListarRecentesAsync()
    {
        var retiradas = await _retiradaRepository.ListarRecentesAsync();

        return retiradas
            .Where(r => r.Funcionario != null)
            .Select(r => new FuncionarioDto
            {
                Id = r.Funcionario.Id,
                NomeCompleto = r.Funcionario.NomeCompleto,
                Matricula = r.Funcionario.Matricula,
                CodigoBarras = r.Funcionario.CodigoBarras,
                Telefone = r.Funcionario.Telefone,
                Setor = r.Funcionario.Setor,
                Status = "Retirado"
            })
            .ToList();
    }

    public async Task ConfirmarRetiradaAsync(int funcionarioId)
    {
        var funcionario = await _funcionarioRepository.BuscarPorIdAsync(funcionarioId);

        if (funcionario is null)
            throw new Exception("Funcionário não encontrado.");

        var cesta = await _cestaRepository.ObterCestaAtivaAsync();

        if (cesta is null)
            throw new Exception("Nenhuma cesta ativa encontrada.");

        var dto = new RetiradaCestaDto
        {
            CodigoBarras = funcionario.CodigoBarras,
            Id = cesta.Id
        };

        await RegistrarRetiradaAsync(dto);
    }
}