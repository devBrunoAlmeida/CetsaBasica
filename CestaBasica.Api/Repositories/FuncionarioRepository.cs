using CestaBasica.Api.Data;
using CestaBasica.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CestaBasica.Api.Repositories;

public class FuncionarioRepository
{
    private readonly AppDbContext _context;

    public FuncionarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Funcionario> CriarAsync(Funcionario funcionario)
    {
        _context.Funcionarios.Add(funcionario);
        await _context.SaveChangesAsync();

        return funcionario;
    }

    public async Task<List<Funcionario>> ListarAsync()
    {
        return await _context.Funcionarios.ToListAsync();
    }
    public async Task<Funcionario?> BuscarPorCodigoBarrasAsync(string codigoBarras)
    {
        return await _context.Funcionarios
            .FirstOrDefaultAsync(f => f.CodigoBarras == codigoBarras);
    }

    public async Task<Funcionario> AtualizarAsync(Funcionario funcionario)
    {
        _context.Funcionarios.Update(funcionario);
        await _context.SaveChangesAsync();
        return funcionario;
    }

    public async Task<Funcionario?> BuscarPorIdAsync(int id)
    {
        return await _context.Funcionarios.FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task ExcluirAsync(int id)
    {
        var funcionario = await _context.Funcionarios
            .FirstOrDefaultAsync(x => x.Id == id);

        if (funcionario is null)
            throw new Exception("Funcionário não encontrado.");

        _context.Funcionarios.Remove(funcionario);

        await _context.SaveChangesAsync();
    }

    public async Task<List<Funcionario>> BuscaManualAsync(string termo)
    {
        return await _context.Funcionarios
            .Where(f =>
                f.NomeCompleto.ToLower().Contains(termo.ToLower()) ||
                f.Matricula.ToLower().Contains(termo.ToLower()))
            .Take(10)
            .ToListAsync();
    }
    public async Task<List<string>> ListarSetoresAsync()
    {
        return await _context.Funcionarios
            .Where(x => !string.IsNullOrWhiteSpace(x.Setor))
            .Select(x => x.Setor)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();
    }

}
