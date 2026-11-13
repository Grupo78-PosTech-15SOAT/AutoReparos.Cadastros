using AutoReparos.Cadastros.Application.Auth.Services.Interfaces;
using AutoReparos.Cadastros.Application.Veiculos.DTOs.Response;
using AutoReparos.Cadastros.Domain.Clientes.Repositories;
using AutoReparos.Cadastros.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace AutoReparos.Cadastros.IntegrationTests.Features.PortalCliente;

[Collection("IntegrationTests")]
public class PortalClienteIntegrationTests(CustomWebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    /// <summary>
    /// Gera um token JWT de cliente localmente consultando o ID real do cliente seedado no banco.
    /// Usa o mesmo emissor RS256 que o endpoint interno do Cadastros usa para a AuthLambda.
    /// </summary>
    private async Task<string> ObterTokenClienteAsync(string cpf, string email)
    {
        using var scope = Factory.Services.CreateScope();
        var clienteRepo = scope.ServiceProvider.GetRequiredService<IClienteRepository>();
        var cliente = await clienteRepo.GetByDocumentoOrEmail(cpf, email)
            ?? throw new InvalidOperationException($"Cliente com CPF {cpf} não encontrado no banco de testes.");

        return scope.ServiceProvider.GetRequiredService<IJwtService>().GenerateToken(cliente);
    }

    [Fact(DisplayName = "GET /api/clientes/meus-veiculos - Cliente deve visualizar apenas seus veículos")]
    public async Task GetMeusVeiculos_ComTokenCliente_DeveRetornarApenasVeiculosDoCliente()
    {
        // Arrange: Leandro Tavares (CPF 97632180044) possui o Gol (Placa ABC1D23)
        var token = await ObterTokenClienteAsync("97632180044", "leandro.silva@email.com");
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/clientes/meus-veiculos");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var veiculos = await response.Content.ReadFromJsonAsync<List<VeiculoDto>>();

        veiculos.Should().NotBeNull();
        veiculos.Should().HaveCount(1);
        veiculos!.First().Placa.Should().Be("ABC1D23");
        veiculos.First().Modelo.Should().Be("Gol");
    }

    [Fact(DisplayName = "Segurança - Token de cliente deve receber 403 Forbidden ao tentar acessar rotas operacionais da oficina")]
    public async Task RotasOperacionais_ComTokenCliente_DeveRetornar403Forbidden()
    {
        // Arrange: Token do cliente Leandro
        var token = await ObterTokenClienteAsync("97632180044", "leandro.silva@email.com");

        // 1. Tentativa de listar todos os clientes da oficina
        using var reqClientes = new HttpRequestMessage(HttpMethod.Get, "/api/clientes");
        reqClientes.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var respClientes = await Client.SendAsync(reqClientes);
        respClientes.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact(DisplayName = "Segurança - Requisições anônimas aos endpoints do portal devem receber 401 Unauthorized")]
    public async Task EndpointsPortal_SemToken_DeveRetornar401Unauthorized()
    {
        // 1. Meus Veículos sem token
        var respVeiculos = await Client.GetAsync("/api/clientes/meus-veiculos");
        respVeiculos.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
