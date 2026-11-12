using Amazon.SQS;
using Amazon.SQS.Model;
using AutoReparos.Cadastros.Application.Notificacoes;
using AutoReparos.Cadastros.Application.Notificacoes.Interfaces;
using Microsoft.Extensions.Options;

namespace AutoReparos.Cadastros.Infrastructure.Services
{
    /// <summary>
    /// Fila SQS própria do Cadastros (long polling).
    /// </summary>
    public class SqsFilaNotificacoes(IAmazonSQS sqs, IOptions<NotificacoesOptions> options) : IFilaNotificacoes
    {
        public async Task<IReadOnlyList<MensagemFila>> ReceberAsync(CancellationToken cancellationToken)
        {
            var resposta = await sqs.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = options.Value.FilaUrl,
                // Uma por vez: as esperas do backoff de uma mensagem não consomem a visibilidade das outras do lote
                MaxNumberOfMessages = 1,
                WaitTimeSeconds = 20,
                VisibilityTimeout = VisibilidadeSegundos(options.Value)
            }, cancellationToken);

            return (resposta.Messages ?? []).Select(m => new MensagemFila(m.MessageId, m.Body, m.ReceiptHandle)).ToList();
        }

        /// <summary>
        /// Cobre todas as esperas do backoff (base, 2x base, ...) mais 30 s por tentativa de envio; sem isso o SQS reentregaria a mensagem
        /// enquanto ela ainda é processada e o consumidor a enviaria em duplicidade ou à DLQ fora de hora.
        /// </summary>
        public static int VisibilidadeSegundos(NotificacoesOptions opcoes)
        {
            var tentativas = Math.Max(1, opcoes.MaxTentativas);
            var esperas = opcoes.AtrasoBase.TotalSeconds * (Math.Pow(2, tentativas - 1) - 1);
            return (int)Math.Ceiling(esperas + 30 * tentativas);
        }

        public Task ExcluirAsync(MensagemFila mensagem, CancellationToken cancellationToken) =>
            sqs.DeleteMessageAsync(options.Value.FilaUrl, mensagem.Recibo, cancellationToken);
    }

    /// <summary>
    /// DLQ própria do Cadastros.
    /// </summary>
    public class SqsFilaMortaNotificacoes(IAmazonSQS sqs, IOptions<NotificacoesOptions> options) : IFilaMortaNotificacoes
    {
        public Task EnviarAsync(MensagemFila mensagem, string motivo, CancellationToken cancellationToken) =>
            sqs.SendMessageAsync(new SendMessageRequest
            {
                QueueUrl = options.Value.DlqUrl,
                MessageBody = mensagem.Corpo,
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    ["motivo"] = new() { DataType = "String", StringValue = motivo.Length > 250 ? motivo[..250] : motivo }
                }
            }, cancellationToken);
    }
}
