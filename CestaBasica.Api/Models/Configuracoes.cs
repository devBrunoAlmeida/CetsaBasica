namespace CestaBasica.Api.Models;

public class Configuracoes
{
    public int Id { get; set; }

    public bool WhatsAppHabilitado { get; set; }
    public bool SmsHabilitado { get; set; }
    public bool EnvioAutomaticoLembretes { get; set; }

    public int TempoSessaoMinutos { get; set; }

    public bool EncerrarPorInatividade { get; set; }

    public string FrequenciaBackup { get; set; } = "Diario";

    public bool PermitirCadastroOperadores { get; set; }

    public bool BloqueioTentativasInvalidas { get; set; }

    public string EvolutionBaseUrl { get; set; } = "http://localhost:8080";
    public string EvolutionApiKey { get; set; } = string.Empty;
    public string EvolutionInstance { get; set; } = "cesta-basica";
    public string EvolutionTelefone { get; set; } = string.Empty;
}