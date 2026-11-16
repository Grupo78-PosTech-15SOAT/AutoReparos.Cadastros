using AutoReparos.Cadastros.Infrastructure.Identity.Services;

namespace AutoReparos.Cadastros.Api.Endpoints
{
    public static class OidcEndpoint
    {
        /// <summary>
        /// Publica a descoberta OIDC e o JWKS (públicos por definição do OIDC).
        /// </summary>
        public static void MapOidcEndpoints(this WebApplication app)
        {
            app.MapGet("/.well-known/openid-configuration", (ChaveAssinaturaJwt chave) => Results.Ok(new
            {
                issuer = chave.Issuer,
                jwks_uri = $"{chave.Issuer}/.well-known/jwks.json",
                id_token_signing_alg_values_supported = new[] { "RS256" },
                subject_types_supported = new[] { "public" },
                response_types_supported = new[] { "id_token" }
            }))
            .WithName("OpenIdConfiguration")
            .WithTags("OIDC")
            .AllowAnonymous();

            app.MapGet("/.well-known/jwks.json", (ChaveAssinaturaJwt chave) => Results.Ok(chave.ObterJwks()))
            .WithName("Jwks")
            .WithTags("OIDC")
            .AllowAnonymous();
        }
    }
}
