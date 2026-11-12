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
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 20
            }, cancellationToken);

            return (resposta.Messages ?? []).Select(m => new MensagemFila(m.MessageId, m.Body, m.ReceiptHandle)).ToList();
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
