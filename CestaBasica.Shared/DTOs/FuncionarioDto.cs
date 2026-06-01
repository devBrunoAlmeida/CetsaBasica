namespace CestaBasica.Shared.DTOs;
using System.ComponentModel.DataAnnotations;

public class FuncionarioDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nome completo é obrigatório.")]
    public string NomeCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Matrícula é obrigatória.")]
    public string Matricula { get; set; } = string.Empty;

    [Required(ErrorMessage = "Código de barras é obrigatório.")]
    public string CodigoBarras { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefone é obrigatório.")]
    public string Telefone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Setor é obrigatório.")]
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