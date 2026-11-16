using Amazon.SQS;
using Amazon.SQS.Model;
using AutoReparos.Cadastros.Application.Notificacoes;
using AutoReparos.Cadastros.Application.Notificacoes.Interfaces;
using AutoReparos.Cadastros.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace AutoReparos.Cadastros.Application.Tests.Notificacoes
{
    /// <summary>
    /// Adaptadores SQS do consumidor de notificações (C7 e C8): o mapeamento da fila, da DLQ e a visibilidade da mensagem.
    /// </summary>
    public class SqsNotificacoesTests
    {
        private const string FilaUrl = "https://sqs.test/000000000000/cadastros-notificacoes";
        private const string DlqUrl = "https://sqs.test/000000000000/cadastros-notificacoes-dlq";

        private readonly IAmazonSQS _sqs = Substitute.For<IAmazonSQS>();
        private readonly NotificacoesOptions _opcoes = new()
        {
            FilaUrl = FilaUrl,
            DlqUrl = DlqUrl,
            MaxTentativas = 4,
            AtrasoBase = TimeSpan.FromSeconds(2)
        };

        [Fact(DisplayName = "A visibilidade cobre as esperas do backoff e o envio de cada tentativa")]
        public void VisibilidadeSegundos_CobreBackoffEEnvio()
        {
            // esperas 2 + 4 + 8 = 14 s, mais 30 s por tentativa (4) = 134 s
            SqsFilaNotificacoes.VisibilidadeSegundos(_opcoes).Should().Be(134);
            SqsFilaNotificacoes.VisibilidadeSegundos(new NotificacoesOptions { MaxTentativas = 1, AtrasoBase = TimeSpan.FromSeconds(2) }).Should().Be(30);
        }

        [Fact(DisplayName = "Receber pede uma mensagem por vez, com a visibilidade calculada, e mapeia o corpo e o recibo")]
        public async Task Receber_PedeUmaPorVezEMapeia()
        {
            ReceiveMessageRequest? enviada = null;
            _sqs.ReceiveMessageAsync(Arg.Do<ReceiveMessageRequest>(r => enviada = r), Arg.Any<CancellationToken>())
                .Returns(new ReceiveMessageResponse { Messages = [new Message { MessageId = "m1", Body = "{\"a\":1}", ReceiptHandle = "recibo-1" }] });
            var fila = new SqsFilaNotificacoes(_sqs, Options.Create(_opcoes));

            var mensagens = await fila.ReceberAsync(CancellationToken.None);

            enviada!.QueueUrl.Should().Be(FilaUrl);
            enviada.MaxNumberOfMessages.Should().Be(1);
            enviada.VisibilityTimeout.Should().Be(134);
            mensagens.Should().ContainSingle().Which.Should().Be(new MensagemFila("m1", "{\"a\":1}", "recibo-1"));
        }

        [Fact(DisplayName = "Excluir apaga a mensagem da fila original pelo recibo")]
        public async Task Excluir_ApagaPeloRecibo()
        {
            var fila = new SqsFilaNotificacoes(_sqs, Options.Create(_opcoes));

            await fila.ExcluirAsync(new MensagemFila("m1", "corpo", "recibo-1"), CancellationToken.None);

            await _sqs.Received(1).DeleteMessageAsync(FilaUrl, "recibo-1", Arg.Any<CancellationToken>());
        }

        [Fact(DisplayName = "A DLQ recebe o corpo original e o motivo, na fila de mortos")]
        public async Task Dlq_RecebeCorpoEMotivo()
        {
            SendMessageRequest? enviada = null;
            _sqs.SendMessageAsync(Arg.Do<SendMessageRequest>(r => enviada = r), Arg.Any<CancellationToken>()).Returns(new SendMessageResponse());
            var dlq = new SqsFilaMortaNotificacoes(_sqs, Options.Create(_opcoes));

            await dlq.EnviarAsync(new MensagemFila("m1", "corpo-original", "recibo-1"), new string('x', 400), CancellationToken.None);

            enviada!.QueueUrl.Should().Be(DlqUrl);
            enviada.MessageBody.Should().Be("corpo-original");
            enviada.MessageAttributes["motivo"].StringValue.Should().HaveLength(250);
        }
    }
}
