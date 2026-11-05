using CestaBasica.Api.Data;
using CestaBasica.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CestaBasica.Api.Repositories;

public class UsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario> CriarAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task<Usuario?> BuscarPorEmailAsync(string email)
{
    return await _context.Usuarios
        .FirstOrDefaultAsync(x => x.Email == email);
}

    public async Task<List<Usuario>> ListarAsync()
    {
        return await _context.Usuarios
            .OrderBy(u => u.Id)
            .ToListAsync();
    }

    public async Task<Usuario?> BuscarPorIdAsync(int id)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Usuario> AtualizarAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task ExcluirAsync(int id)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
        if (usuario is null)
            throw new Exception("Usuário não encontrado.");

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();
    }
}