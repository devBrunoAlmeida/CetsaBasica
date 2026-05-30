namespace CestaBasica.Shared.DTOs;

public class ImportacaoResultadoDto
{
    public int TotalLidos { get; set; }
    public int TotalImportados { get; set; }
    public int TotalInvalidos { get; set; }
    public List<RegistroImportacaoDto> Registros { get; set; } = new();
}

public class RegistroImportacaoDto
{
    public int Linha { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public string CodigoBarras { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Setor { get; set; } = string.Empty;
    public bool Valido { get; set; }
    public string Erro { get; set; } = string.Empty;
}

public class HistoricoImportacaoDto
{
    public int Id { get; set; }
    public string NomeArquivo { get; set; } = string.Empty;
    public DateTime DataImportacao { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public int QuantidadeRegistros { get; set; }
}