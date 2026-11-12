using AutoReparos.Cadastros.Application.Clientes.DTOs.Response;
using AutoReparos.Cadastros.Application.Shared;
using AutoReparos.Cadastros.Application.Veiculos.DTOs.Response;
using AutoReparos.Cadastros.Domain.Usuarios.Enums;
using AutoReparos.Cadastros.IntegrationTests.Infrastructure;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace AutoReparos.Cadastros.IntegrationTests.Features.Propriedade;

public class ConsultaClienteVeiculoPorJwtOuChaveInterna(CustomWebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    private const string ChaveInterna = "chave_interna_dummy_para_testes";

    public static TheoryData<string> Prefixos => new() { "/api", "/api/v1" };

    private async Task<(Guid ClienteId, Guid VeiculoId)> ObterIdsSeedadosAsync()
    {
        await AuthenticateAsync();
        var clientes = await Client.GetFromJsonAsync<PagedResult<ClienteDto>>("/api/clientes?pageNumber=1&pageSize=10");
        var veiculos = await Client.GetFromJsonAsync<PagedResult<VeiculoDto>>("/api/veiculos?pageNumber=1&pageSize=10");
        Client.DefaultRequestHeaders.Authorization = null;
        return (clientes!.Items.First().Id, veiculos!.Items.First().Id);
    }

    [Theory(DisplayName = "C2 - usuário autenticado consulta cliente e veículo por id")]
    [MemberData(nameof(Prefixos))]
    public async Task UsuarioAutenticado_Consulta_RetornaOk(string prefixo)
    {
        var (clienteId, veiculoId) = await ObterIdsSeedadosAsync();
        AuthenticateAsRole(ETipoUsuario.Atendente);

        var cliente = await Client.GetAsync($"{prefixo}/clientes/{clienteId}");
        var veiculo = await Client.GetAsync($"{prefixo}/veiculos/{veiculoId}");

        cliente.StatusCode.Should().Be(HttpStatusCode.OK);
        (await cliente.Content.ReadFromJsonAsync<ClienteDto>())!.Id.Should().Be(clienteId);
        veiculo.StatusCode.Should().Be(HttpStatusCode.OK);
        (await veiculo.Content.ReadFromJsonAsync<VeiculoDto>())!.Id.Should().Be(veiculoId);
    }

    [Theory(DisplayName = "C2 - sem usuário, a chave interna consulta cliente e veículo")]
    [MemberData(nameof(Prefixos))]
    public async Task SemUsuario_ComChaveInterna_RetornaOk(string prefixo)
    {
        var (clienteId, veiculoId) = await ObterIdsSeedadosAsync();
        Client.DefaultRequestHeaders.Add("X-Chave-Interna", ChaveInterna);

        (await Client.GetAsync($"{prefixo}/clientes/{clienteId}")).StatusCode.Should().Be(HttpStatusCode.OK);
        (await Client.GetAsync($"{prefixo}/veiculos/{veiculoId}")).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory(DisplayName = "C2 - sem usuário e sem chave interna, ou com chave errada, recebe 401")]
    [MemberData(nameof(Prefixos))]
    public async Task SemCredencial_OuChaveErrada_Retorna401(string prefixo)
    {
        var (clienteId, veiculoId) = await ObterIdsSeedadosAsync();

        (await Client.GetAsync($"{prefixo}/clientes/{clienteId}")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await Client.GetAsync($"{prefixo}/veiculos/{veiculoId}")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        Client.DefaultRequestHeaders.Add("X-Chave-Interna", "chave-errada");
        (await Client.GetAsync($"{prefixo}/clientes/{clienteId}")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await Client.GetAsync($"{prefixo}/veiculos/{veiculoId}")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory(DisplayName = "C2 - token de cliente (portal) recebe 403 na consulta por id")]
    [MemberData(nameof(Prefixos))]
    public async Task TokenDeCliente_Retorna403(string prefixo)
    {
        var (clienteId, veiculoId) = await ObterIdsSeedadosAsync();
        var token = await ObterTokenClienteAsync();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        (await Client.GetAsync($"{prefixo}/clientes/{clienteId}")).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await Client.GetAsync($"{prefixo}/veiculos/{veiculoId}")).StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory(DisplayName = "C2 - cliente ou veículo inexistente retorna 404")]
    [MemberData(nameof(Prefixos))]
    public async Task Inexistente_Retorna404(string prefixo)
    {
        AuthenticateAsRole(ETipoUsuario.Atendente);

        (await Client.GetAsync($"{prefixo}/clientes/{Guid.NewGuid()}")).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await Client.GetAsync($"{prefixo}/veiculos/{Guid.NewGuid()}")).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<string> ObterTokenClienteAsync()
    {
        await AuthenticateAsync();
        var alvo = (await Client.GetFromJsonAsync<PagedResult<ClienteDto>>("/api/clientes?pageNumber=1&pageSize=1"))!.Items.First();
        Client.DefaultRequestHeaders.Authorization = null;

        Client.DefaultRequestHeaders.Add("X-Chave-Interna", ChaveInterna);
        var ok = await Client.PostAsJsonAsync("/internal/clientes/autenticar", new { cpf = alvo.Documento, email = alvo.Email });
        Client.DefaultRequestHeaders.Remove("X-Chave-Interna");
        ok.StatusCode.Should().Be(HttpStatusCode.OK);
        return (await ok.Content.ReadFromJsonAsync<ClienteAutenticadoDto>())!.Token;
    }
}
