using AutoReparos.Cadastros.IntegrationTests.Infrastructure;
using AutoReparos.Cadastros.Infrastructure.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AutoReparos.Cadastros.IntegrationTests.Features.Propriedade;

public class CadastrosNaoExpoeRotasNemTabelasMigradas(CustomWebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    private static readonly string[] AreasRemovidas = ["ordem-servico", "ordens-servico", "orcamento", "servicos", "insumos", "dashboard"];
    private static readonly string[] TabelasRemovidas = ["OrdensServico", "OrdensServicoServicos", "OrdensServicoInsumos", "Servicos", "Insumos"];

    [Fact(DisplayName = "C1 - Cadastros não expõe rotas de OS, orçamento, catálogo, insumos ou dashboard")]
    public void CadastrosNaoExpoeRotasMigradas()
    {
        var rotas = Services.GetServices<EndpointDataSource>()
            .SelectMany(d => d.Endpoints).OfType<RouteEndpoint>()
            .Select(e => e.RoutePattern.RawText ?? string.Empty).ToList();

        rotas.Should().NotBeEmpty();
        rotas.Where(r => AreasRemovidas.Any(a => r.Contains(a, StringComparison.OrdinalIgnoreCase))).Should().BeEmpty();
    }

    [Fact(DisplayName = "C1 - Cadastros não mantém tabelas de OS, orçamento, catálogo ou insumos")]
    public async Task CadastrosNaoMantemTabelasMigradas()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var existentes = await db.Database
            .SqlQuery<string>($"SELECT table_name AS \"Value\" FROM information_schema.tables WHERE table_schema = 'public'")
            .ToListAsync();

        existentes.Should().Contain("Clientes").And.Contain("Veiculos");
        existentes.Intersect(TabelasRemovidas).Should().BeEmpty();
    }
}
