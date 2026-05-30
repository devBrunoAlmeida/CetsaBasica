namespace CestaBasica.Api.Models;

public class HistoricoImportacao
{
    public int Id { get; set; }
    public string NomeArquivo { get; set; } = string.Empty;
    public DateTime DataImportacao { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public int QuantidadeRegistros { get; set; }
}