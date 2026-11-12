using AutoReparos.Cadastros.IntegrationTests.Infrastructure;
using FluentAssertions;
using System.Text.Json;
using Xunit;

namespace AutoReparos.Cadastros.IntegrationTests.Features.Notificacoes;

public class NotificacaoUmaVezPorEventId(CustomWebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    public static TheoryData<string> TiposComNotificacao => new()
    {
        "orcamento.gerado.v1", "orcamento.aprovado.v1", "orcamento.recusado.v1",
        "execucao.concluida.v1",
        "pagamento.aprovado.v1", "pagamento.recusado.v1",
        "saga.lembrete.v1"
    };

    [Theory(DisplayName = "C7 - evento de orçamento, conclusão, pagamento ou lembrete notifica o cliente uma única vez por eventId")]
    [MemberData(nameof(TiposComNotificacao))]
    public async Task EventoNotificaUmaUnicaVez(string tipo)
    {
        var teste = new ConsumidorDeTeste(Services);
        var cliente = await teste.ObterClienteSeedadoAsync();
        var eventId = Guid.NewGuid();

        teste.Fila.Publicar(ConsumidorDeTeste.Evento(eventId, tipo, cliente.Id));
        await teste.ConsumirAsync();
        // Entrega repetida da mesma mensagem (SQS é at-least-once)
        teste.Fila.Publicar(ConsumidorDeTeste.Evento(eventId, tipo, cliente.Id));
        await teste.ConsumirAsync();

        teste.Email.Enviados.Should().ContainSingle().Which.Email.Should().Be(cliente.Email.Endereco);
        teste.Fila.Excluidas.Should().HaveCount(2, "a duplicada também sai da fila");
        teste.Fila.NaDlq.Should().BeEmpty();
        (await teste.FoiRegistradaAsync(eventId)).Should().BeTrue();
    }

    [Fact(DisplayName = "C7 - eventIds diferentes geram uma notificação cada")]
    public async Task EventIdsDiferentes_GeramUmaCada()
    {
        var teste = new ConsumidorDeTeste(Services);
        var cliente = await teste.ObterClienteSeedadoAsync();

        teste.Fila.Publicar(ConsumidorDeTeste.Evento(Guid.NewGuid(), "orcamento.gerado.v1", cliente.Id));
        teste.Fila.Publicar(ConsumidorDeTeste.Evento(Guid.NewGuid(), "orcamento.gerado.v1", cliente.Id));
        await teste.ConsumirAsync();

        teste.Email.Enviados.Should().HaveCount(2);
    }

    [Fact(DisplayName = "C7 - o envelope do SNS e o payload aninhado são lidos")]
    public async Task EnvelopeSnsEPayloadAninhado_SaoLidos()
    {
        var teste = new ConsumidorDeTeste(Services);
        var cliente = await teste.ObterClienteSeedadoAsync();
        var eventId = Guid.NewGuid();
        var interno = JsonSerializer.Serialize(new
        {
            eventId,
            tipo = "pagamento.aprovado.v1",
            payload = new { clienteId = cliente.Id, ordemServicoId = Guid.NewGuid() }
        });

        teste.Fila.Publicar(JsonSerializer.Serialize(new { Type = "Notification", Message = interno }));
        await teste.ConsumirAsync();

        teste.Email.Enviados.Should().ContainSingle();
        (await teste.FoiRegistradaAsync(eventId)).Should().BeTrue();
    }

    [Fact(DisplayName = "C7 - tipo que não gera notificação é descartado sem e-mail")]
    public async Task TipoSemNotificacao_NaoEnviaEmail()
    {
        var teste = new ConsumidorDeTeste(Services);
        var cliente = await teste.ObterClienteSeedadoAsync();
        var eventId = Guid.NewGuid();

        teste.Fila.Publicar(ConsumidorDeTeste.Evento(eventId, "ordem-servico.criada.v1", cliente.Id));
        await teste.ConsumirAsync();

        teste.Email.Enviados.Should().BeEmpty();
        teste.Fila.Excluidas.Should().ContainSingle();
        (await teste.FoiRegistradaAsync(eventId)).Should().BeFalse();
    }
}
