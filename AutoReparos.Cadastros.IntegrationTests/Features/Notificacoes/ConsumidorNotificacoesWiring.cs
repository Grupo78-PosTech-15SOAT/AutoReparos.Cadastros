using AutoReparos.Cadastros.Application.Notificacoes;
using AutoReparos.Cadastros.Application.Notificacoes.Interfaces;
using AutoReparos.Cadastros.Infrastructure.Services;
using AutoReparos.Cadastros.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace AutoReparos.Cadastros.IntegrationTests.Features.Notificacoes;

/// <summary>
/// O consumidor de notificações está ligado no host real: os serviços resolvem do contêiner e, sem fila configurada, o
/// serviço em segundo plano termina sem tentar falar com a AWS.
/// </summary>
public class ConsumidorNotificacoesWiring(CustomWebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    [Fact(DisplayName = "C7 - o host resolve o consumidor, a fila e a DLQ do SQS e o serviço em segundo plano")]
    public void Host_ResolveOConsumidor()
    {
        using var scope = Services.CreateScope();

        scope.ServiceProvider.GetRequiredService<ConsumirFilaNotificacoesUseCase>().Should().NotBeNull();
        scope.ServiceProvider.GetRequiredService<IFilaNotificacoes>().Should().BeOfType<SqsFilaNotificacoes>();
        scope.ServiceProvider.GetRequiredService<IFilaMortaNotificacoes>().Should().BeOfType<SqsFilaMortaNotificacoes>();
        scope.ServiceProvider.GetRequiredService<IEmailSender>().Should().BeOfType<SendGridEmailSender>();
        Services.GetServices<IHostedService>().Should().ContainSingle(s => s is ConsumidorNotificacoesService);
    }

    [Fact(DisplayName = "C7 - sem Notificacoes:FilaUrl o consumidor fica desligado e termina de imediato")]
    public async Task SemFila_ConsumidorTerminaDeImediato()
    {
        var servico = Services.GetServices<IHostedService>().OfType<ConsumidorNotificacoesService>().Single();

        var inicio = servico.StartAsync(CancellationToken.None);

        (await Task.WhenAny(inicio, Task.Delay(TimeSpan.FromSeconds(5)))).Should().BeSameAs(inicio);
        await servico.StopAsync(CancellationToken.None);
    }
}
