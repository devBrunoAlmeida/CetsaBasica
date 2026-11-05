namespace CestaBasica.Shared.DTOs;

public class WhatsappConfigDto
{
    public string BaseUrl { get; set; } = "http://localhost:8080";
    public string ApiKey { get; set; } = string.Empty;
    public string InstanceName { get; set; } = "cesta-basica";
    public string Telefone { get; set; } = string.Empty;
}

public class WhatsappStatusDto
{
    public bool Configurado { get; set; }
    public bool Conectado { get; set; }
    public string Estado { get; set; } = "nao_configurado";
    public string BaseUrl { get; set; } = string.Empty;
    public string InstanceName { get; set; } = string.Empty;
    public string TelefoneConfigurado { get; set; } = string.Empty;
    public string? NumeroConectado { get; set; }
    public string? PerfilNome { get; set; }
    public string? FotoUrl { get; set; }
    public string? ApiKeyMascarada { get; set; }
    public string? Mensagem { get; set; }
}

public class WhatsappConectarRequestDto
{
    public string? Telefone { get; set; }
    public bool UsarCodigoPareamento { get; set; }
}

public class WhatsappConexaoResultadoDto
{
    public bool Conectado { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? QrCodeBase64 { get; set; }
    public string? PairingCode { get; set; }
    public string? Mensagem { get; set; }
}

public class WhatsappTesteEnvioDto
{
    public string Telefone { get; set; } = string.Empty;
    public string? Mensagem { get; set; }
}
