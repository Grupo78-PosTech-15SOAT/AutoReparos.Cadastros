using AutoReparos.Cadastros.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace AutoReparos.Cadastros.Infrastructure.Identity.Services;

/// <summary>
/// Chave RSA ativa do Cadastros: assina os tokens (privada) e publica o JWKS (somente a parte pública).
/// </summary>
public sealed class ChaveAssinaturaJwt
{
    public string Kid { get; }
    public string Issuer { get; }
    public RsaSecurityKey ChavePrivada { get; }
    public RsaSecurityKey ChavePublica { get; }

    public ChaveAssinaturaJwt(IOptions<JwtSettings> options)
    {
        var settings = options.Value;

        if (string.IsNullOrWhiteSpace(settings.ChavePrivadaPem))
            throw new InvalidOperationException($"'{JwtSettings.SectionName}:ChavePrivadaPem' é obrigatória para assinar tokens RS256.");

        if (string.IsNullOrWhiteSpace(settings.Issuer))
            throw new InvalidOperationException($"'{JwtSettings.SectionName}:Issuer' é obrigatório.");

        var rsa = RSA.Create();
        // Variáveis de ambiente e Secrets costumam trazer o PEM em uma linha, com "\n" literal no lugar das quebras
        rsa.ImportFromPem(settings.ChavePrivadaPem.Replace("\\n", "\n"));

        var rsaPublica = RSA.Create();
        rsaPublica.ImportParameters(rsa.ExportParameters(false));

        ChavePublica = new RsaSecurityKey(rsaPublica);
        Kid = string.IsNullOrWhiteSpace(settings.KeyId)
            ? Base64UrlEncoder.Encode(ChavePublica.ComputeJwkThumbprint())
            : settings.KeyId;

        ChavePublica.KeyId = Kid;
        ChavePrivada = new RsaSecurityKey(rsa) { KeyId = Kid };
        Issuer = settings.Issuer.TrimEnd('/');
    }

    /// <summary>
    /// Conjunto de chaves públicas (JWKS). Nunca inclui parâmetros privados.
    /// </summary>
    public object ObterJwks()
    {
        var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(ChavePublica);
        return new
        {
            keys = new[]
            {
                new { kty = jwk.Kty, use = "sig", alg = SecurityAlgorithms.RsaSha256, kid = Kid, n = jwk.N, e = jwk.E }
            }
        };
    }
}
