using AutoReparos.Cadastros.Application.Auth.DTOs.Request;
using AutoReparos.Cadastros.Application.Auth.DTOs.Response;
using AutoReparos.Cadastros.Domain.Usuarios.Enums;
using AutoReparos.Cadastros.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;
using Xunit;

namespace AutoReparos.Cadastros.IntegrationTests.Features.Propriedade;

public class JwtValidadoPeloJwksComKid(CustomWebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    private async Task<string> LogarAsync()
    {
        var resposta = await Client.PostAsJsonAsync("/api/auth/login", new LoginRequestDto(DefaultAdminEmail, DefaultAdminPassword));
        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        return (await resposta.Content.ReadFromJsonAsync<LoginResponseDto>())!.Token;
    }

    [Fact(DisplayName = "C3 - um serviço valida o JWT pelo JWKS publicado, sem segredo compartilhado")]
    public async Task ServicoValidaTokenPeloJwks()
    {
        var token = await LogarAsync();

        // Um serviço só conhece o Authority: descobre o JWKS pelo documento OIDC
        var descoberta = await Client.GetFromJsonAsync<JsonElement>("/.well-known/openid-configuration");
        var issuer = descoberta.GetProperty("issuer").GetString()!;
        var jwksUri = new Uri(descoberta.GetProperty("jwks_uri").GetString()!);
        var jwks = new JsonWebKeySet(await Client.GetStringAsync(jwksUri.AbsolutePath));

        var validacao = new TokenValidationParameters
        {
            ValidIssuer = issuer,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = jwks.GetSigningKeys(),
            ValidAlgorithms = [SecurityAlgorithms.RsaSha256]
        };

        new JwtSecurityTokenHandler().ValidateToken(token, validacao, out var validado);

        validado.Should().BeOfType<JwtSecurityToken>().Which.Header.Alg.Should().Be("RS256");
    }

    [Fact(DisplayName = "C3 - o token leva o kid da chave ativa publicada no JWKS")]
    public async Task TokenLevaKidDaChaveAtiva()
    {
        var token = await LogarAsync();
        var jwks = await Client.GetFromJsonAsync<JsonElement>("/.well-known/jwks.json");

        var kidDoJwks = jwks.GetProperty("keys").EnumerateArray().Single().GetProperty("kid").GetString();
        var kidDoToken = new JwtSecurityTokenHandler().ReadJwtToken(token).Header.Kid;

        kidDoToken.Should().NotBeNullOrWhiteSpace().And.Be(kidDoJwks);
    }

    [Fact(DisplayName = "C3 - token assinado com outra chave é recusado pelo Cadastros")]
    public async Task TokenDeOutraChave_Retorna401()
    {
        using var rsa = RSA.Create(2048);
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = "http://cadastros.test",
            Expires = DateTime.UtcNow.AddHours(1),
            Subject = new System.Security.Claims.ClaimsIdentity([new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, ETipoUsuario.Administrador.ToString())]),
            SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        };
        var handler = new JwtSecurityTokenHandler();
        var falso = handler.WriteToken(handler.CreateToken(descriptor));
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", falso);

        (await Client.GetAsync("/api/clientes")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
