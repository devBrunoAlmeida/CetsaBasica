using CestaBasica.Api.Models;
using CestaBasica.Api.Repositories;
using CestaBasica.Shared.DTOs;

namespace CestaBasica.Api.Services;

public class NotificacaoService
{
    private readonly FuncionarioRepository _funcionarioRepository;
    private readonly NotificacaoRepository _notificacaoRepository;

    public NotificacaoService(
        FuncionarioRepository funcionarioRepository,
        NotificacaoRepository notificacaoRepository)
    {
        _funcionarioRepository = funcionarioRepository;
        _notificacaoRepository = notificacaoRepository;
    }

    public async Task<Notificacao> EnviarAsync(NotificacaoRequestDto dto)
    {
        var funcionario = await _funcionarioRepository.BuscarPorIdAsync(dto.FuncionarioId);

        if (funcionario == null)
            throw new Exception("Funcionário não encontrado.");

        if (string.IsNullOrWhiteSpace(funcionario.Telefone))
            throw new Exception("Funcionário sem telefone cadastrado.");

        if (string.IsNullOrWhiteSpace(dto.Mensagem))
            throw new Exception("Mensagem é obrigatória.");

        var notificacao = new Notificacao
        {
            FuncionarioId = funcionario.Id,
            Canal = dto.Canal,
            Telefone = funcionario.Telefone,
            Titulo = dto.Titulo,
            Mensagem = dto.Mensagem,
            Status = "Enviado",
            ProtocoloExterno = Guid.NewGuid().ToString(),
            DataEnvio = DateTime.UtcNow
        };

        return await _notificacaoRepository.CriarAsync(notificacao);
    }

    public async Task<List<Notificacao>> ListarAsync()
    {
        return await _notificacaoRepository.ListarAsync();
    }
}