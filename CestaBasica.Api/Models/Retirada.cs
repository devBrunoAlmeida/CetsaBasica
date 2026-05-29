namespace CestaBasica.Api.Models;

public class Retirada
{
    public int Id { get; set; }
    public int FuncionarioId { get; set; }
    public Funcionario Funcionario { get; set; } = null!;
    public int CestaId { get; set; }
    public Cesta Cesta { get; set; } = null!;
    public DateTime DataRetirada { get; set; } = DateTime.UtcNow;
}
