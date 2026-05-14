namespace CestaBasica.Api.DTOs;

public class DashboardDto
{
    public int TotalFuncionarios { get; set; }
    public int TotalCestas { get; set; }
    public int TotalRetiradas { get; set; }
    public int TotalNotificacoes { get; set; }

    public int TotalCestasDisponiveis { get; set; }
}