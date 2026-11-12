using AutoReparos.Cadastros.Application.Notificacoes;
using AutoReparos.Cadastros.Application.Notificacoes.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoReparos.Cadastros.Infrastructure.Services
{
    public class TaskAtrasador : IAtrasador
    {
        public Task AguardarAsync(TimeSpan atraso, CancellationToken cancellationToken) => Task.Delay(atraso, cancellationToken);
    }

    /// <summary>
    /// Lê a fila de notificações enquanto a aplicação estiver de pé.
    /// </summary>
    public class ConsumidorNotificacoesService(
        IServiceScopeFactory scopeFactory,
        IOptions<NotificacoesOptions> options,
        ILogger<ConsumidorNotificacoesService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (string.IsNullOrWhiteSpace(options.Value.FilaUrl))
            {
                logger.LogInformation("Notificacoes:FilaUrl não configurada; consumidor de notificações desligado.");
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    await scope.ServiceProvider.GetRequiredService<ConsumirFilaNotificacoesUseCase>().ExecuteAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Falha ao consumir a fila de notificações; nova tentativa em 5 segundos.");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }
    }
}
