namespace CestaBasica.Api.DTOs;

public class CestaCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int QuantidadeDisponivel { get; set; }
}