namespace AutoReparos.Cadastros.Domain.Notificacoes.Entities
{
    /// <summary>
    /// Registro de que a notificação de um evento já foi enviada; a chave é o <c>eventId</c> (deduplicação).
    /// </summary>
    public class NotificacaoEnviada
    {
        public Guid EventId { get; private set; }
        public string Tipo { get; private set; } = null!;
        public Guid ClienteId { get; private set; }
        public DateTime EnviadoEm { get; private set; }

        /// <summary>
        /// Construtor para uso do Entity Framework
        /// </summary>
        protected NotificacaoEnviada() { }

        public NotificacaoEnviada(Guid eventId, string tipo, Guid clienteId, DateTime enviadoEm)
        {
            EventId = eventId;
            Tipo = tipo;
            ClienteId = clienteId;
            EnviadoEm = enviadoEm;
        }
    }
}
