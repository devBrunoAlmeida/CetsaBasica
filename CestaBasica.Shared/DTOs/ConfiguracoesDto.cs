namespace CestaBasica.Shared.DTOs;

public class ConfiguracoesDto
{
    public bool WhatsAppHabilitado { get; set; } = true;
    public bool SmsHabilitado { get; set; } = true;
    public bool EnvioAutomaticoLembretes { get; set; }

    public int TempoSessaoMinutos { get; set; } = 60;
    public bool EncerrarPorInatividade { get; set; } = true;

    public string FrequenciaBackup { get; set; } = "Diario";

    public bool PermitirCadastroOperadores { get; set; } = true;

    public bool BloqueioTentativasInvalidas { get; set; } = true;

    public string EvolutionBaseUrl { get; set; } = "http://localhost:8080";
    public string EvolutionApiKey { get; set; } = string.Empty;
    public string EvolutionInstance { get; set; } = "cesta-basica";
    public string EvolutionTelefone { get; set; } = string.Empty;
}