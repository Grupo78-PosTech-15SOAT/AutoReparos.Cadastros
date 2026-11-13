using AutoReparos.Cadastros.Application.Clientes.DTOs.Request;
using AutoReparos.Cadastros.Domain.Usuarios.Enums;
using AutoReparos.Cadastros.IntegrationTests.Infrastructure;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace AutoReparos.Cadastros.IntegrationTests.Features.Propriedade;

public class EndpointInternoRecusaSemChave(CustomWebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    private static readonly AutenticarClienteDto Corpo = new("97632180044", "leandro.silva@email.com");

    [Fact(DisplayName = "C6 - sem X-Chave-Interna o endpoint interno retorna 401")]
    public async Task SemChave_Retorna401()
    {
        (await Client.PostAsJsonAsync("/internal/clientes/autenticar", Corpo)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact(DisplayName = "C6 - X-Chave-Interna inválida retorna 401")]
    public async Task ChaveInvalida_Retorna401()
    {
        Client.DefaultRequestHeaders.Add("X-Chave-Interna", "chave-errada");

        (await Client.PostAsJsonAsync("/internal/clientes/autenticar", Corpo)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact(DisplayName = "C6 - JWT de usuário não substitui a chave interna")]
    public async Task JwtDeUsuario_SemChave_Retorna401()
    {
        AuthenticateAsRole(ETipoUsuario.Administrador);

        (await Client.PostAsJsonAsync("/internal/clientes/autenticar", Corpo)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact(DisplayName = "C6 - a chave interna não abre as rotas operacionais da oficina")]
    public async Task ChaveInterna_NaoAbreRotasOperacionais()
    {
        Client.DefaultRequestHeaders.Add("X-Chave-Interna", "chave_interna_dummy_para_testes");

        (await Client.GetAsync("/api/clientes")).StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden);
    }
}
