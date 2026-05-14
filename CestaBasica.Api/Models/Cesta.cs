namespace CestaBasica.Api.Models;

public class Cesta
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public int QuantidadeDisponivel { get; set; }

    public bool Ativa { get; set; } = true;

    public DateTime CriadoEm { get; set; } = DateTime.Now;
}