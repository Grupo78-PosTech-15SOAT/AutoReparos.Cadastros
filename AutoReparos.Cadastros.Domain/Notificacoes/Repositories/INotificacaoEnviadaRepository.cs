using AutoReparos.Cadastros.Domain.Notificacoes.Entities;

namespace AutoReparos.Cadastros.Domain.Notificacoes.Repositories
{
    public interface INotificacaoEnviadaRepository
    {
        Task<bool> Existe(Guid eventId);

        /// <summary>
        /// Registra o envio. Devolve <c>false</c> quando o <c>eventId</c> já estava registrado (outra instância se adiantou).
        /// </summary>
        Task<bool> Registrar(NotificacaoEnviada notificacao);
    }
}
