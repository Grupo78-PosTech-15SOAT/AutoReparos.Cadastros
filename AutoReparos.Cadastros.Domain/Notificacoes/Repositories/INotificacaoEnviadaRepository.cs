using AutoReparos.Cadastros.Domain.Notificacoes.Entities;

namespace AutoReparos.Cadastros.Domain.Notificacoes.Repositories
{
    public interface INotificacaoEnviadaRepository
    {
        Task<bool> Existe(Guid eventId);

        /// <summary>
        /// Executa <paramref name="acao"/> segurando uma trava exclusiva do <paramref name="eventId"/> (vale entre instâncias),
        /// para que duas entregas simultâneas do mesmo evento não enviem o e-mail duas vezes.
        /// </summary>
        Task<T> ComTrava<T>(Guid eventId, Func<Task<T>> acao);

        /// <summary>
        /// Registra o envio. Devolve <c>false</c> quando o <c>eventId</c> já estava registrado (outra instância se adiantou).
        /// </summary>
        Task<bool> Registrar(NotificacaoEnviada notificacao);
    }
}
