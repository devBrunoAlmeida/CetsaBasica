namespace CestaBasica.Api.Models;

public class Funcionario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public string CodigoBarras { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Setor { get; set; } = string.Empty;
    public bool Retirado { get; set; } = false;
    public DateTime? DataRetirada { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}