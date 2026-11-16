using System.Text.Json;

namespace AutoReparos.Cadastros.Application.Notificacoes
{
    /// <summary>
    /// Evento que dispara uma notificação ao cliente (orçamento, conclusão, pagamento ou lembrete).
    /// </summary>
    public record EventoNotificacao(Guid EventId, string Tipo, Guid ClienteId, Guid? OrdemServicoId)
    {
        public static readonly IReadOnlyCollection<string> TiposAceitos =
        [
            "orcamento.gerado.v1", "orcamento.aprovado.v1", "orcamento.recusado.v1",
            "execucao.concluida.v1",
            "pagamento.aprovado.v1", "pagamento.recusado.v1",
            "saga.lembrete.v1"
        ];

        /// <summary>
        /// Lê o envelope comum (<c>eventId</c>, <c>tipo</c>, <c>clienteId</c>, <c>ordemServicoId</c>), no topo ou dentro de <c>payload</c>,
        /// com ou sem o invólucro de notificação do SNS. Devolve <c>null</c> para tipos que não geram notificação.
        /// </summary>
        /// <exception cref="FormatException">O corpo não é um envelope válido.</exception>
        public static EventoNotificacao? Ler(string corpo)
        {
            try
            {
                using var doc = JsonDocument.Parse(corpo);
                var raiz = doc.RootElement;

                if (raiz.TryGetProperty("Type", out var tipoSns) && tipoSns.GetString() == "Notification"
                    && raiz.TryGetProperty("Message", out var mensagem) && mensagem.ValueKind == JsonValueKind.String)
                {
                    return Ler(mensagem.GetString()!);
                }

                var tipo = LerTexto(raiz, "tipo") ?? throw new FormatException("Envelope sem 'tipo'.");
                if (!TiposAceitos.Contains(tipo))
                    return null;

                var eventId = LerGuid(raiz, "eventId") ?? throw new FormatException("Envelope sem 'eventId'.");
                var carga = raiz.TryGetProperty("payload", out var p) && p.ValueKind == JsonValueKind.Object ? p : raiz;
                var clienteId = LerGuid(carga, "clienteId") ?? LerGuid(raiz, "clienteId")
                    ?? throw new FormatException("Evento sem 'clienteId'.");
                var ordemServicoId = LerGuid(carga, "ordemServicoId") ?? LerGuid(raiz, "correlacaoId");

                return new EventoNotificacao(eventId, tipo, clienteId, ordemServicoId);
            }
            catch (JsonException ex)
            {
                throw new FormatException("Corpo da mensagem não é JSON válido.", ex);
            }
        }

        private static string? LerTexto(JsonElement e, string nome) =>
            e.TryGetProperty(nome, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;

        private static Guid? LerGuid(JsonElement e, string nome) =>
            Guid.TryParse(LerTexto(e, nome), out var id) ? id : null;
    }
}
