public class RetiradaCestaDto
{
    public int Id { get; set; }
    public int FuncionarioId { get; set; }
    public int LoteCestaId { get; set; }
    public DateTime DataRetirada { get; set; }
    public string CodigoBarras { get; set; } = string.Empty;
    public string Observacao { get; set; } = string.Empty;
}