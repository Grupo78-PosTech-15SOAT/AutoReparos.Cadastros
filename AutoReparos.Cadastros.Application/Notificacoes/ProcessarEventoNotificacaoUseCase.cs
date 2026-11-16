using AutoReparos.Cadastros.Application.Notificacoes.Interfaces;
using AutoReparos.Cadastros.Domain.Clientes.Repositories;
using AutoReparos.Cadastros.Domain.Notificacoes.Entities;
using AutoReparos.Cadastros.Domain.Notificacoes.Repositories;

namespace AutoReparos.Cadastros.Application.Notificacoes
{
    /// <summary>
    /// A falha não se resolve com nova tentativa (corpo inválido ou cliente inexistente): a mensagem vai direto para a DLQ.
    /// </summary>
    public class FalhaPermanenteNotificacaoException(string message, Exception? inner = null) : Exception(message, inner);

    public enum ResultadoNotificacao
    {
        Enviada,
        Duplicada,
        Ignorada
    }

    public class ProcessarEventoNotificacaoUseCase(
        INotificacaoEnviadaRepository enviadas,
        IClienteRepository clientes,
        IEmailSender emailSender,
        TimeProvider relogio)
    {
        public async Task<ResultadoNotificacao> ExecuteAsync(string corpo, CancellationToken cancellationToken)
        {
            EventoNotificacao? evento;
            try
            {
                evento = EventoNotificacao.Ler(corpo);
            }
            catch (FormatException ex)
            {
                throw new FalhaPermanenteNotificacaoException(ex.Message, ex);
            }

            if (evento is null)
                return ResultadoNotificacao.Ignorada;

            // A trava por eventId cobre Existe, envio e registro: entregas simultâneas do mesmo evento não duplicam o e-mail
            return await enviadas.ComTrava(evento.EventId, async () =>
            {
                if (await enviadas.Existe(evento.EventId))
                    return ResultadoNotificacao.Duplicada;

                var cliente = await clientes.GetById(evento.ClienteId)
                    ?? throw new FalhaPermanenteNotificacaoException($"Cliente {evento.ClienteId} do evento {evento.EventId} não existe.");

                var (assunto, texto) = MontarMensagem(evento, cliente.Nome);
                await emailSender.EnviarAsync(cliente.Email.Endereco, cliente.Nome, assunto, texto, cancellationToken);

                // O eventId só é registrado depois do envio: uma falha anterior permite nova tentativa.
                var registrada = await enviadas.Registrar(new NotificacaoEnviada(evento.EventId, evento.Tipo, cliente.Id, relogio.GetUtcNow().UtcDateTime));
                return registrada ? ResultadoNotificacao.Enviada : ResultadoNotificacao.Duplicada;
            });
        }

        private static (string Assunto, string Texto) MontarMensagem(EventoNotificacao evento, string nome)
        {
            var os = evento.OrdemServicoId is { } id ? $" #{id.ToString()[..8].ToUpperInvariant()}" : string.Empty;
            return evento.Tipo switch
            {
                "orcamento.gerado.v1" => ($"Orçamento da Ordem de Serviço{os}", $"Olá, {nome}! O orçamento da sua ordem de serviço{os} está pronto para sua análise."),
                "orcamento.aprovado.v1" => ($"Orçamento aprovado{os}", $"Olá, {nome}! Recebemos a aprovação do orçamento da ordem de serviço{os}."),
                "orcamento.recusado.v1" => ($"Orçamento recusado{os}", $"Olá, {nome}! O orçamento da ordem de serviço{os} foi recusado."),
                "execucao.concluida.v1" => ($"Serviço concluído{os}", $"Olá, {nome}! O serviço da ordem{os} foi concluído."),
                "pagamento.aprovado.v1" => ($"Pagamento aprovado{os}", $"Olá, {nome}! O pagamento da ordem de serviço{os} foi aprovado."),
                "pagamento.recusado.v1" => ($"Pagamento recusado{os}", $"Olá, {nome}! O pagamento da ordem de serviço{os} foi recusado."),
                _ => ($"Lembrete da ordem de serviço{os}", $"Olá, {nome}! Há uma ação pendente na sua ordem de serviço{os}.")
            };
        }
    }
}
