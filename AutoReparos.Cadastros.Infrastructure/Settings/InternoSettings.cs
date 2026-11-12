namespace AutoReparos.Cadastros.Infrastructure.Settings;

/// <summary>
/// Configuração da chave interna (<c>X-Chave-Interna</c>) usada por OS e AuthLambda.
/// </summary>
public class InternoSettings
{
    public const string SectionName = "Interno";

    /// <summary>
    /// Segredo compartilhado, vindo de um Secret do Kubernetes.
    /// </summary>
    public string Chave { get; set; } = string.Empty;
}
