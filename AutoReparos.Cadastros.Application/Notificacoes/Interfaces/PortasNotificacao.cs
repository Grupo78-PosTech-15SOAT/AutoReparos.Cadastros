namespace AutoReparos.Cadastros.Application.Notificacoes.Interfaces
{
    /// <summary>
    /// Mensagem recebida da fila de notificações.
    /// </summary>
    public record MensagemFila(string Id, string Corpo, string Recibo);

    /// <summary>
    /// Fila SQS própria do Cadastros, assinante dos tópicos SNS dos produtores.
    /// </summary>
    public interface IFilaNotificacoes
    {
        Task<IReadOnlyList<MensagemFila>> ReceberAsync(CancellationToken cancellationToken);
        Task ExcluirAsync(MensagemFila mensagem, CancellationToken cancellationToken);
    }

    /// <summary>
    /// DLQ própria do Cadastros, para mensagens cujo envio esgotou as tentativas.
    /// </summary>
    public interface IFilaMortaNotificacoes
    {
        Task EnviarAsync(MensagemFila mensagem, string motivo, CancellationToken cancellationToken);
    }

    public interface IEmailSender
    {
        Task EnviarAsync(string destinatarioEmail, string destinatarioNome, string assunto, string texto, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Espera entre tentativas; separado para que os testes não esperem de verdade.
    /// </summary>
    public interface IAtrasador
    {
        Task AguardarAsync(TimeSpan atraso, CancellationToken cancellationToken);
    }
}
