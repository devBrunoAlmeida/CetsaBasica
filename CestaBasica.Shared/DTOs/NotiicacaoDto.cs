namespace CestaBasica.Shared.DTOs;

public class NotificacaoRequestDto
{
    public int FuncionarioId { get; set; }
    public string Canal { get; set; } = "WhatsApp";
    public string Titulo { get; set; } = "Retirada de Cesta Básica";
    public string Mensagem { get; set; } = string.Empty;
    public DateTime DataEnvio { get; set; } = DateTime.UtcNow;
}