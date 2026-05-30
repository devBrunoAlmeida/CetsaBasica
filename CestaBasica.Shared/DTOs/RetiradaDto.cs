public class RetiradaCestaDto
{
    public int Id { get; set; }
    public int FuncionarioId { get; set; }
    public int LoteCestaId { get; set; }
    public DateTime DataRetirada { get; set; }
    public string CodigoBarras { get; set; } = string.Empty;
    public string Observacao { get; set; } = string.Empty;
}

public class RetiradaRecenteDto
{
    public string NomeCompleto { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public string Setor { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime DataRetirada { get; set; }
}