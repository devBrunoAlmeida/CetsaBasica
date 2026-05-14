namespace CestaBasica.Api.DTOs;

public class NotificacaoRequestDto
{
    public int FuncionarioId { get; set; }
    public string Canal { get; set; } = "WhatsApp";
    public string Mensagem { get; set; } = string.Empty;
}