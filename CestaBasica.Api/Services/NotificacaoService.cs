using CestaBasica.Api.Models;
using CestaBasica.Api.Repositories;
using CestaBasica.Shared.DTOs;

namespace CestaBasica.Api.Services;

public class NotificacaoService
{
    private readonly FuncionarioRepository _funcionarioRepository;
    private readonly NotificacaoRepository _notificacaoRepository;
    private readonly EvolutionApiService _evolutionApiService;

    public NotificacaoService(
        FuncionarioRepository funcionarioRepository,
        NotificacaoRepository notificacaoRepository,
        EvolutionApiService evolutionApiService)
    {
        _funcionarioRepository = funcionarioRepository;
        _notificacaoRepository = notificacaoRepository;
        _evolutionApiService = evolutionApiService;
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
            Status = "Pendente",
            DataEnvio = DateTime.UtcNow
        };

        try
        {
            var resultado = await _evolutionApiService.EnviarTextoAsync(
                funcionario.Telefone,
                dto.Mensagem
            );

            notificacao.Status = "Enviado";
            notificacao.ProtocoloExterno = resultado;
            notificacao.DataEnvio = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            notificacao.Status = "Falhou";
            notificacao.Erro = ex.Message;
        }

        return await _notificacaoRepository.CriarAsync(notificacao);
    }

    public async Task<List<Notificacao>> ListarAsync()
    {
        return await _notificacaoRepository.ListarAsync();
    }

    public Task<WhatsappStatusDto> ObterStatusWhatsappAsync()
        => _evolutionApiService.ObterStatusAsync();

    public Task<WhatsappStatusDto> ConfigurarWhatsappAsync(WhatsappConfigDto dto)
        => _evolutionApiService.SalvarConfiguracaoAsync(dto);

    public Task<WhatsappConexaoResultadoDto> ConectarWhatsappAsync(WhatsappConectarRequestDto dto)
        => _evolutionApiService.ConectarAsync(dto);

    public Task<WhatsappStatusDto> DesconectarWhatsappAsync()
        => _evolutionApiService.DesconectarAsync();

    public async Task<string> EnviarTesteWhatsappAsync(WhatsappTesteEnvioDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Telefone))
            throw new Exception("Informe um número de telefone com DDD para enviar o teste.");

        var texto = string.IsNullOrWhiteSpace(dto.Mensagem)
            ? "✅ Olá! Teste de conexão realizado com sucesso pelo Sistema de Gestão de Cestas Básicas."
            : dto.Mensagem;

        return await _evolutionApiService.EnviarTextoAsync(dto.Telefone, texto);
    }
}
