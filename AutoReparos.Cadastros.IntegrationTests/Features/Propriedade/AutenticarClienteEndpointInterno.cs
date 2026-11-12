using AutoReparos.Cadastros.Application.Clientes.DTOs.Request;
using AutoReparos.Cadastros.Application.Clientes.DTOs.Response;
using AutoReparos.Cadastros.Application.Shared;
using AutoReparos.Cadastros.IntegrationTests.Infrastructure;
using FluentAssertions;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace AutoReparos.Cadastros.IntegrationTests.Features.Propriedade;

public class AutenticarClienteEndpointInterno(CustomWebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    private const string ChaveInterna = "chave_interna_dummy_para_testes";

    private async Task<ClienteDto> ObterClienteSeedadoAsync()
    {
        await AuthenticateAsync();
        var cliente = (await Client.GetFromJsonAsync<PagedResult<ClienteDto>>("/api/clientes?pageNumber=1&pageSize=1"))!.Items.First();
        Client.DefaultRequestHeaders.Authorization = null;
        Client.DefaultRequestHeaders.Add("X-Chave-Interna", ChaveInterna);
        return cliente;
    }

    [Fact(DisplayName = "C5 - CPF e e-mail corretos devolvem o cliente e o JWT RS256 do Cadastros")]
    public async Task CpfEEmailCorretos_RetornaClienteEToken()
    {
        var alvo = await ObterClienteSeedadoAsync();

        var resposta = await Client.PostAsJsonAsync("/internal/clientes/autenticar", new AutenticarClienteDto(alvo.Documento, alvo.Email));

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        var corpo = (await resposta.Content.ReadFromJsonAsync<ClienteAutenticadoDto>())!;
        corpo.Cliente.Id.Should().Be(alvo.Id);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(corpo.Token);
        jwt.Header.Alg.Should().Be("RS256");
        jwt.Header.Kid.Should().NotBeNullOrWhiteSpace();
        jwt.Claims.Should().Contain(c => c.Type == "cpf" && c.Value == alvo.Documento);
    }

    [Fact(DisplayName = "C5 - CPF com máscara é aceito")]
    public async Task CpfComMascara_RetornaOk()
    {
        var alvo = await ObterClienteSeedadoAsync();
        var d = alvo.Documento;
        var mascarado = $"{d[..3]}.{d[3..6]}.{d[6..9]}-{d[9..]}";

        var resposta = await Client.PostAsJsonAsync("/internal/clientes/autenticar", new AutenticarClienteDto(mascarado, alvo.Email));

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "C5 - e-mail diferente do cadastro retorna 404")]
    public async Task EmailDiferente_Retorna404()
    {
        var alvo = await ObterClienteSeedadoAsync();

        var resposta = await Client.PostAsJsonAsync("/internal/clientes/autenticar", new AutenticarClienteDto(alvo.Documento, "outro@email.com"));

        resposta.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "C5 - CPF inexistente retorna 404")]
    public async Task CpfInexistente_Retorna404()
    {
        await ObterClienteSeedadoAsync();

        var resposta = await Client.PostAsJsonAsync("/internal/clientes/autenticar", new AutenticarClienteDto("00000000000", "x@email.com"));

        resposta.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
