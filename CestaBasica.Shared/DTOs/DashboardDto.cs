namespace CestaBasica.Shared.DTOs;

public class DashboardDto
{
    public int TotalFuncionarios { get; set; }
    public int TotalCestas { get; set; }
    public int TotalRetiradas { get; set; }
    public int TotalNotificacoes { get; set; }
    public int TotalCestasDisponiveis { get; set; }

    public List<SetorDashboardDto> DistribuicaoPorSetor { get; set; } = new();
    public List<EvolucaoRetiradaDto> EvolucaoRetiradas { get; set; } = new();
    public List<UltimaRetiradaDto> UltimasRetiradas { get; set; } = new();
    public List<NotificacaoRecenteDto> NotificacoesRecentes { get; set; } = new();
}

public class SetorDashboardDto
{
    public string Setor { get; set; } = string.Empty;
    public decimal Porcentagem { get; set; }
}

public class EvolucaoRetiradaDto
{
    public string Mes { get; set; } = string.Empty;
    public int Total { get; set; }
}

public class UltimaRetiradaDto
{
    public string Funcionario { get; set; } = string.Empty;
    public string Setor { get; set; } = string.Empty;
    public DateTime? Data { get; set; } 
    public string Status { get; set; } = string.Empty;
}

public class NotificacaoRecenteDto
{
    public string Titulo { get; set; } = "Retirada  Cesta Básica";
    public string Mensagem { get; set; } = string.Empty;
    public DateTime? DataEnvio { get; set; } 
}