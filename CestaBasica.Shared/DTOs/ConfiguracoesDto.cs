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
}