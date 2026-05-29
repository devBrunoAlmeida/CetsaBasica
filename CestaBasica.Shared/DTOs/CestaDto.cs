namespace CestaBasica.Shared.DTOs;

public class CestaDto
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public bool Ativa { get; set; } = true;

    public int QuantidadeDisponivel { get; set; }
    public DateTime DataCriacao { get; set; }
}

