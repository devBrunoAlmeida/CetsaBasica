using BCrypt.Net;
using CestaBasica.Api.Models;
using CestaBasica.Api.Repositories;
using CestaBasica.Shared.DTOs;

namespace CestaBasica.Api.Services;

public class UsuarioService
{
    private readonly UsuarioRepository _repo;

    public UsuarioService(UsuarioRepository repo)
    {
        _repo = repo;
    }

    public async Task<Usuario> CriarAsync(UsuarioCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new Exception("Nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new Exception("E-mail é obrigatório.");

        if (string.IsNullOrWhiteSpace(dto.Senha))
            throw new Exception("Senha é obrigatória.");

        var usuarioExistente =
            await _repo.BuscarPorEmailAsync(dto.Email);

        if (usuarioExistente != null)
            throw new Exception("Já existe usuário com este e-mail.");

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
            Perfil = dto.Perfil,
            Ativo = true
        };

        return await _repo.CriarAsync(usuario);
    }

    public async Task<List<Usuario>> ListarAsync()
    {
        return await _repo.ListarAsync();
    }

    public async Task<Usuario> LoginAsync(LoginDto dto)
    {
        var usuario =
            await _repo.BuscarPorEmailAsync(dto.Email);

        if (usuario == null)
            throw new Exception("Usuário ou senha inválidos.");

        var senhaValida =
            BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash);

        if (!senhaValida)
            throw new Exception("Usuário ou senha inválidos.");

        if (!usuario.Ativo)
            throw new Exception("Usuário inativo.");

        if (usuario.Perfil != dto.Perfil)
            throw new Exception("Perfil inválido.");

        return usuario;
    }
}