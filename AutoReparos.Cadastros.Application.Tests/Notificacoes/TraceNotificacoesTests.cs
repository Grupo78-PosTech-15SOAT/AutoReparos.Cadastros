using System.Diagnostics;
using System.Text.Json;
using AutoReparos.Cadastros.Application.Notificacoes;
using AutoReparos.Cadastros.Application.Notificacoes.Interfaces;
using AutoReparos.Cadastros.Domain.Clientes.Entities;
using AutoReparos.Cadastros.Domain.Clientes.Repositories;
using AutoReparos.Cadastros.Domain.Notificacoes.Entities;
using AutoReparos.Cadastros.Domain.Notificacoes.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace AutoReparos.Cadastros.Application.Tests.Notificacoes
{
    /// <summary>
    /// Continuação do trace W3C no consumo das notificações (C5 e C7 de infra-ci-observabilidade).
    /// </summary>
    public sealed class TraceNotificacoesTests : IDisposable
    {
        private const string TraceId = "4bf92f3577b34da6a3ce929d0e0e4736";
        private const string SpanIdPai = "00f067aa0ba902b7";
        private const string Tipo = "orcamento.gerado.v1";

        private readonly Guid _eventId = Guid.NewGuid();
        private readonly Guid _ordemServicoId = Guid.NewGuid();
        private readonly Cliente _cliente = new("João Silva", "52998224725", "11999999999", "joao@teste.com");

        private readonly IFilaNotificacoes _fila = Substitute.For<IFilaNotificacoes>();
        private readonly IFilaMortaNotificacoes _dlq = Substitute.For<IFilaMortaNotificacoes>();
        private readonly INotificacaoEnviadaRepository _enviadas = Substitute.For<INotificacaoEnviadaRepository>();
        private readonly IClienteRepository _clientes = Substitute.For<IClienteRepository>();
        private readonly IEmailSender _email = Substitute.For<IEmailSender>();

        private readonly List<Activity> _atividades = [];
        private readonly ActivityListener _listener;

        public TraceNotificacoesTests()
        {
            _listener = new ActivityListener
            {
                ShouldListenTo = fonte => fonte.Name == "AutoReparos.Cadastros.Mensageria",
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
                ActivityStopped = a => { lock (_atividades) _atividades.Add(a); }
            };
            ActivitySource.AddActivityListener(_listener);

            _clientes.GetById(_cliente.Id).Returns(_cliente);
            _enviadas.ComTrava(Arg.Any<Guid>(), Arg.Any<Func<Task<ResultadoNotificacao>>>())
                .Returns(c => c.Arg<Func<Task<ResultadoNotificacao>>>()!());
            _enviadas.Registrar(Arg.Any<NotificacaoEnviada>()).Returns(true);
        }

        public void Dispose() => _listener.Dispose();

        [Fact(DisplayName = "Envelope com traceparent válido é processado num span Consumer filho do contexto do envelope")]
        public async Task TraceparentValido_SpanConsumerFilhoDoEnvelope()
        {
            await ConsumirAsync(Envelope(("traceparent", $"00-{TraceId}-{SpanIdPai}-01")));

            AssertSpanFilhoDoEnvelope();
            await AssertNotificadaSemDlqAsync();
        }

        [Fact(DisplayName = "Envelope dentro do invólucro de notificação do SNS continua o mesmo trace")]
        public async Task TraceparentValidoNoInvolucroSns_SpanConsumerFilhoDoEnvelope()
        {
            var envelope = Envelope(("traceparent", $"00-{TraceId}-{SpanIdPai}-01"));
            var sns = JsonSerializer.Serialize(new { Type = "Notification", MessageId = "sns-1", Message = envelope });

            await ConsumirAsync(sns);

            AssertSpanFilhoDoEnvelope();
            await AssertNotificadaSemDlqAsync();
        }

        [Fact(DisplayName = "Envelope sem traceparent é processado como hoje, num span raiz, sem ir para a DLQ")]
        public async Task SemTraceparent_ProcessaNumSpanRaiz()
        {
            await ConsumirAsync(Envelope());

            AssertSpanRaiz();
            await AssertNotificadaSemDlqAsync();
        }

        [Fact(DisplayName = "Envelope com traceparent inválido é processado como hoje, num span raiz, sem ir para a DLQ")]
        public async Task TraceparentInvalido_ProcessaNumSpanRaiz()
        {
            await ConsumirAsync(Envelope(("traceparent", "abc")));

            AssertSpanRaiz();
            await AssertNotificadaSemDlqAsync();
        }

        [Fact(DisplayName = "Envelope com campo desconhecido é processado como hoje, num span raiz, sem ir para a DLQ")]
        public async Task CampoDesconhecido_ProcessaNumSpanRaiz()
        {
            await ConsumirAsync(Envelope(("campoNovo", "qualquer")));

            AssertSpanRaiz();
            await AssertNotificadaSemDlqAsync();
        }

        [Fact(DisplayName = "Uma atividade do ambiente não vira pai do span de uma mensagem sem traceparent")]
        public async Task AtividadeDoAmbiente_NaoViraPai()
        {
            using var fonteAmbiente = new ActivitySource("AutoReparos.Cadastros.Mensageria");
            using var ambiente = fonteAmbiente.StartActivity("ambiente");

            await ConsumirAsync(Envelope());

            AssertSpanRaiz();
        }

        private string Envelope(params (string Nome, string Valor)[] extras)
        {
            var envelope = new Dictionary<string, object>
            {
                ["eventId"] = _eventId.ToString(),
                ["tipo"] = Tipo,
                ["payload"] = new Dictionary<string, string>
                {
                    ["clienteId"] = _cliente.Id.ToString(),
                    ["ordemServicoId"] = _ordemServicoId.ToString()
                }
            };
            foreach (var (nome, valor) in extras)
                envelope[nome] = valor;
            return JsonSerializer.Serialize(envelope);
        }

        private async Task ConsumirAsync(string corpo)
        {
            _fila.ReceberAsync(Arg.Any<CancellationToken>()).Returns([new MensagemFila("m1", corpo, "recibo-1")]);
            var processar = new ProcessarEventoNotificacaoUseCase(_enviadas, _clientes, _email, TimeProvider.System);
            var consumir = new ConsumirFilaNotificacoesUseCase(
                _fila, _dlq, processar, Substitute.For<IAtrasador>(),
                Options.Create(new NotificacoesOptions { MaxTentativas = 1 }),
                NullLogger<ConsumirFilaNotificacoesUseCase>.Instance);

            await consumir.ExecuteAsync(CancellationToken.None);
        }

        private Activity SpanDaMensagem()
        {
            lock (_atividades)
            {
                return _atividades.Should().ContainSingle(a => (string?)a.GetTagItem("messaging.message.id") == _eventId.ToString())
                    .Subject;
            }
        }

        private void AssertSpanFilhoDoEnvelope()
        {
            var span = SpanDaMensagem();
            span.Kind.Should().Be(ActivityKind.Consumer);
            span.DisplayName.Should().Be($"processar {Tipo}");
            span.TraceId.ToHexString().Should().Be(TraceId);
            span.ParentSpanId.ToHexString().Should().Be(SpanIdPai);
            span.GetTagItem("ordem_servico.id").Should().Be(_ordemServicoId.ToString());
        }

        private void AssertSpanRaiz()
        {
            var span = SpanDaMensagem();
            span.Kind.Should().Be(ActivityKind.Consumer);
            span.DisplayName.Should().Be($"processar {Tipo}");
            span.Parent.Should().BeNull();
            span.ParentSpanId.Should().Be(default(ActivitySpanId));
            span.TraceId.ToHexString().Should().NotBe(TraceId);
            span.GetTagItem("ordem_servico.id").Should().Be(_ordemServicoId.ToString());
        }

        private async Task AssertNotificadaSemDlqAsync()
        {
            await _email.Received(1).EnviarAsync("joao@teste.com", "João Silva", Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
            await _fila.Received(1).ExcluirAsync(Arg.Any<MensagemFila>(), Arg.Any<CancellationToken>());
            await _dlq.DidNotReceiveWithAnyArgs().EnviarAsync(default!, default!, default);
        }
    }
}
