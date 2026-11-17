using System.Diagnostics;
using System.Text.Json;

namespace AutoReparos.Cadastros.Application.Notificacoes
{
    /// <summary>
    /// Dados de rastreio de uma mensagem da fila: o tipo, o <c>eventId</c>, o <c>ordemServicoId</c> e o contexto W3C do produtor.
    /// </summary>
    public record MetadadosMensagem(string Tipo, string? EventId, string? OrdemServicoId, ActivityContext? Pai);

    /// <summary>
    /// Continua o trace W3C dos produtores no consumo das notificações (D-65, AD-070).
    /// </summary>
    public static class ContextoTrace
    {
        public const string NomeFonte = "AutoReparos.Cadastros.Mensageria";

        public static readonly ActivitySource Fonte = new(NomeFonte);

        private static readonly MetadadosMensagem Desconhecida = new("desconhecido", null, null, null);

        /// <summary>
        /// Lê <c>traceparent</c> e <c>tracestate</c> da raiz do envelope. Ausente ou inválido devolve <c>null</c>.
        /// </summary>
        public static ActivityContext? Extrair(JsonElement raiz)
        {
            if (raiz.ValueKind != JsonValueKind.Object)
                return null;

            var traceparent = LerTexto(raiz, "traceparent");
            if (traceparent is null)
                return null;

            return ActivityContext.TryParse(traceparent, LerTexto(raiz, "tracestate"), isRemote: true, out var contexto)
                ? contexto
                : null;
        }

        /// <summary>
        /// Lê os metadados de rastreio do corpo, com ou sem o invólucro de notificação do SNS. Nunca lança:
        /// um corpo ilegível só perde o rastreio, e o processamento decide o destino da mensagem como antes.
        /// </summary>
        public static MetadadosMensagem LerMetadados(string corpo)
        {
            try
            {
                using var doc = JsonDocument.Parse(corpo);
                var raiz = doc.RootElement;

                if (raiz.ValueKind == JsonValueKind.Object
                    && LerTexto(raiz, "Type") == "Notification"
                    && LerTexto(raiz, "Message") is { } mensagem)
                {
                    return LerMetadados(mensagem);
                }

                if (raiz.ValueKind != JsonValueKind.Object)
                    return Desconhecida;

                var carga = raiz.TryGetProperty("payload", out var p) && p.ValueKind == JsonValueKind.Object ? p : raiz;
                return new MetadadosMensagem(
                    LerTexto(raiz, "tipo") ?? Desconhecida.Tipo,
                    LerTexto(raiz, "eventId"),
                    LerTexto(raiz, "ordemServicoId") ?? LerTexto(carga, "ordemServicoId"),
                    Extrair(raiz));
            }
            catch (JsonException)
            {
                return Desconhecida;
            }
        }

        private static string? LerTexto(JsonElement e, string nome) =>
            e.TryGetProperty(nome, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
    }
}
