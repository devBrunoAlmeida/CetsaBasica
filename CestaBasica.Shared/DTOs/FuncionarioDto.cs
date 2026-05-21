namespace CestaBasica.Shared.DTOs;

public class FuncionarioDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public string CodigoBarras { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Setor { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}