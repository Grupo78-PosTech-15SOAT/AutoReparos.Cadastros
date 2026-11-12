using AutoReparos.Cadastros.Application.Clientes.DTOs.Request;
using AutoReparos.Cadastros.Application.Clientes.DTOs.Response;
using AutoReparos.Cadastros.Application.Shared;
using AutoReparos.Cadastros.Application.Usuarios.DTOs.Request;
using AutoReparos.Cadastros.Domain.Usuarios.Enums;
using AutoReparos.Cadastros.IntegrationTests.Infrastructure;
using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace AutoReparos.Cadastros.IntegrationTests.Features.Propriedade;

/// <summary>
/// O CRUD de clientes, veículos e usuários mantém os contratos do App, agora também em /api/v1 (D-28).
/// A validação de domínio responde 400 (contrato atual), não 422.
/// </summary>
public class CrudMantidoSemMudanca(CustomWebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    public static TheoryData<string> Prefixos => new() { "/api", "/api/v1" };

    private static ClienteCreateDto NovoCliente()
    {
        var faker = new Faker("pt_BR");
        return new ClienteCreateDto(faker.Person.FullName, faker.Person.Cpf(), faker.Random.ReplaceNumbers("119########"), faker.Internet.Email());
    }

    [Theory(DisplayName = "C9 - ciclo de vida de cliente: 201, 200, 204 e 404 após excluir")]
    [MemberData(nameof(Prefixos))]
    public async Task Cliente_CicloDeVida(string prefixo)
    {
        await AuthenticateAsync();

        var criado = await Client.PostAsJsonAsync($"{prefixo}/clientes", NovoCliente());
        criado.StatusCode.Should().Be(HttpStatusCode.Created);
        var id = (await criado.Content.ReadFromJsonAsync<ClienteDto>())!.Id;

        (await Client.GetAsync($"{prefixo}/clientes/{id}")).StatusCode.Should().Be(HttpStatusCode.OK);
        (await Client.GetAsync($"{prefixo}/clientes?pageNumber=1&pageSize=5")).StatusCode.Should().Be(HttpStatusCode.OK);
        (await Client.PutAsJsonAsync($"{prefixo}/clientes/{id}", new ClienteUpdateDto("Nome Atualizado", "11987654321", "novo@email.com")))
            .StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await Client.DeleteAsync($"{prefixo}/clientes/{id}")).StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await Client.GetAsync($"{prefixo}/clientes/{id}")).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await Client.PutAsJsonAsync($"{prefixo}/clientes/{Guid.NewGuid()}", new ClienteUpdateDto("Nome", "11987654321", "a@b.com")))
            .StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory(DisplayName = "C9 - dados inválidos de cliente retornam 400 (contrato atual)")]
    [MemberData(nameof(Prefixos))]
    public async Task Cliente_DadosInvalidos_Retorna400(string prefixo)
    {
        await AuthenticateAsync();

        var resposta = await Client.PostAsJsonAsync($"{prefixo}/clientes", new ClienteCreateDto("Fulano", "123", "11987654321", "fulano@email.com"));

        resposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory(DisplayName = "C9 - listagens de veículos e usuários respondem 200 nos dois prefixos")]
    [MemberData(nameof(Prefixos))]
    public async Task Listagens_RespondemOk(string prefixo)
    {
        await AuthenticateAsync();

        (await Client.GetAsync($"{prefixo}/veiculos?pageNumber=1&pageSize=5")).StatusCode.Should().Be(HttpStatusCode.OK);
        (await Client.GetAsync($"{prefixo}/usuarios?pageNumber=1&pageSize=5")).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory(DisplayName = "C9 - sem token 401; atendente cria usuário 403")]
    [MemberData(nameof(Prefixos))]
    public async Task Autorizacao_Mantida(string prefixo)
    {
        foreach (var rota in new[] { "clientes", "veiculos", "usuarios" })
            (await Client.GetAsync($"{prefixo}/{rota}")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        AuthenticateAsRole(ETipoUsuario.Atendente);
        var resposta = await Client.PostAsJsonAsync($"{prefixo}/usuarios", new UsuarioCreateDto("Novo Usuario", "novo@oficina.com", "Senha@123", ETipoUsuario.Mecanico));
        resposta.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory(DisplayName = "C9 - usuário inexistente retorna 404")]
    [MemberData(nameof(Prefixos))]
    public async Task Usuario_Inexistente_Retorna404(string prefixo)
    {
        await AuthenticateAsync();

        (await Client.GetAsync($"{prefixo}/usuarios/{Guid.NewGuid()}")).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await Client.GetAsync($"{prefixo}/veiculos/{Guid.NewGuid()}")).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
