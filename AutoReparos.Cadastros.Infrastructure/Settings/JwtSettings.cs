namespace AutoReparos.Cadastros.Infrastructure.Settings;

/// <summary>
/// Configuração da emissão de JWT RS256 pelo Cadastros (D-21, door 2 do plano).
/// </summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    /// <summary>
    /// Emissor (<c>iss</c>) dos tokens e <c>issuer</c> do documento de descoberta OIDC.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Chave privada RSA em PEM (PKCS#8), vinda de um Secret do Kubernetes.
    /// </summary>
    public string ChavePrivadaPem { get; set; } = string.Empty;

    /// <summary>
    /// Identificador da chave ativa (<c>kid</c>). Quando vazio, usa o thumbprint da chave pública.
    /// </summary>
    public string KeyId { get; set; } = string.Empty;

    public int ExpiryHours { get; set; } = 2;
}
