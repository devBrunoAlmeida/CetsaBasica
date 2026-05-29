public class Cesta
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;

    public bool Ativa { get; set; }

    public int QuantidadeDisponivel { get; set; }

    public DateTime DataCriacao { get; set; }
}