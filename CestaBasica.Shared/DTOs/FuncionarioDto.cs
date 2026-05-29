namespace CestaBasica.Shared.DTOs;

public class FuncionarioDto
{
    public int Id { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public string CodigoBarras { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Setor { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;


}
public class HistoricoFuncionarioDto
{
    public string Status { get; set; } = string.Empty;
    public string LotesAtivos { get; set; } = string.Empty;
    public DateTime? UltimaRetirada { get; set; }
    public DateTime? ProximaRetirada { get; set; }
}
public class EventoHistoricoDto
{
    public DateTime DataHora { get; set; }

    public string Tipo { get; set; } = string.Empty;
    // RETIRADA
    // NOTIFICACAO
    // CADASTRO

    public string Titulo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public string Responsavel { get; set; } = string.Empty;
}
public class HistoricoFuncionarioResponse
{
    public FuncionarioDto Funcionario { get; set; } = new();

    public HistoricoFuncionarioDto Historico { get; set; } = new();

    public List<EventoHistoricoDto> Eventos { get; set; } = new();
}