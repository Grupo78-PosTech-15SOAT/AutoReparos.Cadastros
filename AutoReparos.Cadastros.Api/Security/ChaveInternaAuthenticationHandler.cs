using AutoReparos.Cadastros.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

namespace AutoReparos.Cadastros.Api.Security;

/// <summary>
/// Autentica chamadas internas (OS e AuthLambda) pelo cabeçalho <c>X-Chave-Interna</c>.
/// </summary>
public class ChaveInternaAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IOptions<InternoSettings> interno)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string Scheme = "ChaveInterna";
    public const string Cabecalho = "X-Chave-Interna";
    public const string RoleServicoInterno = "ServicoInterno";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Cabecalho, out var enviada))
            return Task.FromResult(AuthenticateResult.NoResult());

        var esperada = interno.Value.Chave;
        if (string.IsNullOrEmpty(esperada) || !ChavesIguais(enviada.ToString(), esperada))
            return Task.FromResult(AuthenticateResult.Fail("Chave interna inválida."));

        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Role, RoleServicoInterno)], Scheme);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme)));
    }

    private static bool ChavesIguais(string a, string b)
    {
        var x = SHA256.HashData(Encoding.UTF8.GetBytes(a));
        var y = SHA256.HashData(Encoding.UTF8.GetBytes(b));
        return CryptographicOperations.FixedTimeEquals(x, y);
    }
}
