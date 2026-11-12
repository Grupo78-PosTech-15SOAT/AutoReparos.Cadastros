using AutoReparos.Cadastros.IntegrationTests.Infrastructure;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace AutoReparos.Cadastros.IntegrationTests.Features.Propriedade;

public class DiscoveryEJwksPublicos(CustomWebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    [Fact(DisplayName = "C4 - discovery OIDC é público e aponta para o JWKS")]
    public async Task Discovery_SemCredencial_RetornaOk()
    {
        var resposta = await Client.GetAsync("/.well-known/openid-configuration");

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        var doc = await resposta.Content.ReadFromJsonAsync<JsonElement>();
        doc.GetProperty("issuer").GetString().Should().Be("http://cadastros.test");
        doc.GetProperty("jwks_uri").GetString().Should().Be("http://cadastros.test/.well-known/jwks.json");
    }

    [Fact(DisplayName = "C4 - JWKS é público e devolve só a chave pública")]
    public async Task Jwks_SemCredencial_DevolveSoChavePublica()
    {
        var resposta = await Client.GetAsync("/.well-known/jwks.json");

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        var corpo = await resposta.Content.ReadAsStringAsync();
        var chave = JsonDocument.Parse(corpo).RootElement.GetProperty("keys").EnumerateArray().Single();

        chave.GetProperty("kty").GetString().Should().Be("RSA");
        chave.GetProperty("alg").GetString().Should().Be("RS256");
        chave.GetProperty("use").GetString().Should().Be("sig");
        chave.TryGetProperty("n", out _).Should().BeTrue();
        chave.TryGetProperty("e", out _).Should().BeTrue();
        foreach (var privado in new[] { "d", "p", "q", "dp", "dq", "qi" })
            chave.TryGetProperty(privado, out _).Should().BeFalse($"o parâmetro privado '{privado}' não pode ser publicado");
    }

    [Fact(DisplayName = "C4 - caminho inexistente em .well-known retorna 404")]
    public async Task CaminhoInexistente_Retorna404()
    {
        (await Client.GetAsync("/.well-known/inexistente")).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
