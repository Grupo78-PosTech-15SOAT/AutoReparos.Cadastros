using AutoReparos.Cadastros.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace AutoReparos.Cadastros.IntegrationTests.Features.Notificacoes;

public class FalhaDeEmailRepeteEVaiParaDlq(CustomWebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    [Fact(DisplayName = "C8 - falha transitória repete com backoff exponencial e só então registra o eventId")]
    public async Task FalhaTransitoria_RepeteEDepoisRegistra()
    {
        var teste = new ConsumidorDeTeste(Services, maxTentativas: 4);
        teste.Email.FalhasIniciais = 2;
        var cliente = await teste.ObterClienteSeedadoAsync();
        var eventId = Guid.NewGuid();

        teste.Fila.Publicar(ConsumidorDeTeste.Evento(eventId, "orcamento.gerado.v1", cliente.Id));
        await teste.ConsumirAsync();

        teste.Email.Chamadas.Should().Be(3);
        teste.Email.Enviados.Should().ContainSingle();
        teste.Atrasador.Esperas.Should().Equal(teste.AtrasoBase, teste.AtrasoBase * 2);
        teste.Fila.NaDlq.Should().BeEmpty();
        teste.Fila.Excluidas.Should().ContainSingle();
        (await teste.FoiRegistradaAsync(eventId)).Should().BeTrue();
    }

    [Fact(DisplayName = "C8 - esgotadas as tentativas, a mensagem vai para a DLQ e o eventId não é registrado")]
    public async Task TentativasEsgotadas_VaiParaDlqSemRegistrar()
    {
        var teste = new ConsumidorDeTeste(Services, maxTentativas: 4);
        teste.Email.FalhasIniciais = int.MaxValue;
        var cliente = await teste.ObterClienteSeedadoAsync();
        var eventId = Guid.NewGuid();
        var corpo = ConsumidorDeTeste.Evento(eventId, "pagamento.recusado.v1", cliente.Id);

        teste.Fila.Publicar(corpo);
        await teste.ConsumirAsync();

        teste.Email.Chamadas.Should().Be(4);
        teste.Atrasador.Esperas.Should().Equal(teste.AtrasoBase, teste.AtrasoBase * 2, teste.AtrasoBase * 4);
        teste.Fila.NaDlq.Should().ContainSingle().Which.Mensagem.Corpo.Should().Be(corpo);
        teste.Fila.Excluidas.Should().ContainSingle("a mensagem original sai da fila depois de ir para a DLQ");
        (await teste.FoiRegistradaAsync(eventId)).Should().BeFalse("o eventId só é registrado depois do envio");
    }

    [Fact(DisplayName = "C8 - uma mensagem na DLQ pode ser reprocessada: o eventId não registrado volta a notificar")]
    public async Task AposDlq_ReprocessamentoNotifica()
    {
        var teste = new ConsumidorDeTeste(Services, maxTentativas: 2);
        teste.Email.FalhasIniciais = 2;
        var cliente = await teste.ObterClienteSeedadoAsync();
        var corpo = ConsumidorDeTeste.Evento(Guid.NewGuid(), "execucao.concluida.v1", cliente.Id);

        teste.Fila.Publicar(corpo);
        await teste.ConsumirAsync();
        teste.Fila.NaDlq.Should().ContainSingle();

        teste.Fila.Publicar(corpo);
        await teste.ConsumirAsync();

        teste.Email.Enviados.Should().ContainSingle();
    }

    [Fact(DisplayName = "C8 - cliente inexistente é falha permanente: vai direto para a DLQ, sem repetir")]
    public async Task ClienteInexistente_VaiDiretoParaDlq()
    {
        var teste = new ConsumidorDeTeste(Services, maxTentativas: 4);

        teste.Fila.Publicar(ConsumidorDeTeste.Evento(Guid.NewGuid(), "orcamento.gerado.v1", Guid.NewGuid()));
        await teste.ConsumirAsync();

        teste.Email.Chamadas.Should().Be(0);
        teste.Atrasador.Esperas.Should().BeEmpty();
        teste.Fila.NaDlq.Should().ContainSingle();
    }

    [Fact(DisplayName = "C8 - mensagem que não é JSON vai direto para a DLQ")]
    public async Task CorpoInvalido_VaiDiretoParaDlq()
    {
        var teste = new ConsumidorDeTeste(Services);

        teste.Fila.Publicar("isto não é json");
        await teste.ConsumirAsync();

        teste.Fila.NaDlq.Should().ContainSingle();
        teste.Atrasador.Esperas.Should().BeEmpty();
    }
}
