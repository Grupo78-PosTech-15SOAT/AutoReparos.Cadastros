using AutoReparos.Cadastros.Application.Notificacoes.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoReparos.Cadastros.Application.Notificacoes
{
    public class NotificacoesOptions
    {
        public const string SectionName = "Notificacoes";

        /// <summary>URL da fila SQS própria do Cadastros. Vazia desliga o consumidor.</summary>
        public string FilaUrl { get; set; } = string.Empty;

        /// <summary>URL da DLQ própria do Cadastros.</summary>
        public string DlqUrl { get; set; } = string.Empty;

        /// <summary>Endpoint alternativo do SQS (LocalStack).</summary>
        public string ServiceUrl { get; set; } = string.Empty;

        /// <summary>Tentativas de envio por mensagem antes de ir para a DLQ.</summary>
        public int MaxTentativas { get; set; } = 4;

        /// <summary>Atraso antes da 2ª tentativa; dobra a cada nova tentativa.</summary>
        public TimeSpan AtrasoBase { get; set; } = TimeSpan.FromSeconds(2);
    }

    public class ConsumirFilaNotificacoesUseCase(
        IFilaNotificacoes fila,
        IFilaMortaNotificacoes dlq,
        ProcessarEventoNotificacaoUseCase processar,
        IAtrasador atrasador,
        IOptions<NotificacoesOptions> options,
        ILogger<ConsumirFilaNotificacoesUseCase> logger)
    {
        /// <summary>
        /// Recebe um lote e processa cada mensagem: sucesso (ou duplicada) exclui da fila; falha repete com backoff exponencial
        /// e, ao esgotar as tentativas, envia para a DLQ própria e exclui da fila original.
        /// </summary>
        public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
        {
            var mensagens = await fila.ReceberAsync(cancellationToken);
            foreach (var mensagem in mensagens)
                await ProcessarMensagemAsync(mensagem, cancellationToken);

            return mensagens.Count;
        }

        private async Task ProcessarMensagemAsync(MensagemFila mensagem, CancellationToken cancellationToken)
        {
            var opcoes = options.Value;
            var maximo = Math.Max(1, opcoes.MaxTentativas);
            Exception? ultima = null;

            for (var tentativa = 1; tentativa <= maximo; tentativa++)
            {
                try
                {
                    await processar.ExecuteAsync(mensagem.Corpo, cancellationToken);
                    await fila.ExcluirAsync(mensagem, cancellationToken);
                    return;
                }
                catch (FalhaPermanenteNotificacaoException ex)
                {
                    ultima = ex;
                    break;
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    ultima = ex;
                    logger.LogWarning(ex, "Falha ao notificar a mensagem {MensagemId} (tentativa {Tentativa}/{Maximo}).", mensagem.Id, tentativa, maximo);

                    if (tentativa < maximo)
                        await atrasador.AguardarAsync(opcoes.AtrasoBase * Math.Pow(2, tentativa - 1), cancellationToken);
                }
            }

            logger.LogError(ultima, "Mensagem {MensagemId} enviada para a DLQ.", mensagem.Id);
            await dlq.EnviarAsync(mensagem, ultima?.Message ?? "falha desconhecida", cancellationToken);
            await fila.ExcluirAsync(mensagem, cancellationToken);
        }
    }
}
