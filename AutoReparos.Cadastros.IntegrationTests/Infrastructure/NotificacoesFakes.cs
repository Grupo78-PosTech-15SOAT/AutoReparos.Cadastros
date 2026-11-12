using AutoReparos.Cadastros.Application.Notificacoes;
using AutoReparos.Cadastros.Application.Notificacoes.Interfaces;
using AutoReparos.Cadastros.Domain.Clientes.Entities;
using AutoReparos.Cadastros.Domain.Clientes.Repositories;
using AutoReparos.Cadastros.Domain.Notificacoes.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AutoReparos.Cadastros.IntegrationTests.Infrastructure;

public class EmailSenderFalso : IEmailSender
{
    public List<(string Email, string Assunto)> Enviados { get; } = [];
    public int Chamadas { get; private set; }

    /// <summary>Número de chamadas iniciais que falham.</summary>
    public int FalhasIniciais { get; set; }

    public Task EnviarAsync(string destinatarioEmail, string destinatarioNome, string assunto, string texto, CancellationToken cancellationToken)
    {
        Chamadas++;
        if (Chamadas <= FalhasIniciais)
            throw new InvalidOperationException("SendGrid indisponível");

        Enviados.Add((destinatarioEmail, assunto));
        return Task.CompletedTask;
    }
}

public class FilaFalsa : IFilaNotificacoes, IFilaMortaNotificacoes
{
    private readonly Queue<MensagemFila> _pendentes = new();
    public List<MensagemFila> Excluidas { get; } = [];
    public List<(MensagemFila Mensagem, string Motivo)> NaDlq { get; } = [];

    public void Publicar(string corpo) => _pendentes.Enqueue(new MensagemFila(Guid.NewGuid().ToString(), corpo, Guid.NewGuid().ToString()));

    public Task<IReadOnlyList<MensagemFila>> ReceberAsync(CancellationToken cancellationToken)
    {
        var lote = _pendentes.ToList();
        _pendentes.Clear();
        return Task.FromResult<IReadOnlyList<MensagemFila>>(lote);
    }

    public Task ExcluirAsync(MensagemFila mensagem, CancellationToken cancellationToken)
    {
        Excluidas.Add(mensagem);
        return Task.CompletedTask;
    }

    public Task EnviarAsync(MensagemFila mensagem, string motivo, CancellationToken cancellationToken)
    {
        NaDlq.Add((mensagem, motivo));
        return Task.CompletedTask;
    }
}

public class AtrasadorFalso : IAtrasador
{
    public List<TimeSpan> Esperas { get; } = [];

    public Task AguardarAsync(TimeSpan atraso, CancellationToken cancellationToken)
    {
        Esperas.Add(atraso);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Monta o consumidor real (casos de uso e repositórios reais, banco real) com fakes só nas bordas: fila, DLQ, e-mail e espera.
/// </summary>
public class ConsumidorDeTeste(IServiceProvider services, int maxTentativas = 3)
{
    public FilaFalsa Fila { get; } = new();
    public EmailSenderFalso Email { get; } = new();
    public AtrasadorFalso Atrasador { get; } = new();
    public TimeSpan AtrasoBase { get; } = TimeSpan.FromSeconds(2);

    public async Task ConsumirAsync()
    {
        using var scope = services.CreateScope();
        var processar = new ProcessarEventoNotificacaoUseCase(
            scope.ServiceProvider.GetRequiredService<INotificacaoEnviadaRepository>(),
            scope.ServiceProvider.GetRequiredService<IClienteRepository>(),
            Email,
            TimeProvider.System);

        var consumidor = new ConsumirFilaNotificacoesUseCase(
            Fila, Fila, processar, Atrasador,
            Options.Create(new NotificacoesOptions { MaxTentativas = maxTentativas, AtrasoBase = AtrasoBase }),
            NullLogger<ConsumirFilaNotificacoesUseCase>.Instance);

        await consumidor.ExecuteAsync(CancellationToken.None);
    }

    public async Task<Cliente> ObterClienteSeedadoAsync()
    {
        using var scope = services.CreateScope();
        var (clientes, _) = await scope.ServiceProvider.GetRequiredService<IClienteRepository>().GetAll(null, 0, 1);
        return clientes.First();
    }

    public async Task<bool> FoiRegistradaAsync(Guid eventId)
    {
        using var scope = services.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<INotificacaoEnviadaRepository>().Existe(eventId);
    }

    public static string Evento(Guid eventId, string tipo, Guid clienteId) => JsonSerializer.Serialize(new
    {
        eventId,
        tipo,
        correlacaoId = Guid.NewGuid(),
        ocorridoEm = DateTime.UtcNow,
        ordemServicoId = Guid.NewGuid(),
        clienteId
    });
}
