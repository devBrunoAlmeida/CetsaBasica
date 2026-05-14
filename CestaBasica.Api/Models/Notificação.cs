namespace CestaBasica.Api.Models;

public class Notificacao
{
    public int Id { get; set; }

    public int FuncionarioId { get; set; }
    public Funcionario Funcionario { get; set; } = null!;

    public string Canal { get; set; } = string.Empty; // SMS ou WhatsApp
    public string Telefone { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public string Status { get; set; } = "Pendente";

    public string? ProtocoloExterno { get; set; }
    public string? Erro { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.Now;
    public DateTime? EnviadoEm { get; set; }
}