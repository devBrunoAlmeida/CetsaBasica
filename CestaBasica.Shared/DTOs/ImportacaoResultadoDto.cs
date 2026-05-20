namespace CestaBasica.Shared.DTOs;

public class ImportacaoResultadoDto
{
    public int TotalImportados { get; set; }
    public List<string> Erros { get; set; } = new();
}