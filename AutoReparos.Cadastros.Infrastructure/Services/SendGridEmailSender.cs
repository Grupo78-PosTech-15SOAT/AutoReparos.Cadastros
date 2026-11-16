using AutoReparos.Cadastros.Application.Notificacoes.Interfaces;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace AutoReparos.Cadastros.Infrastructure.Services
{
    /// <summary>
    /// Envia o e-mail pelo SendGrid (D-29). Qualquer resposta que não seja de sucesso vira exceção para o consumidor repetir.
    /// </summary>
    public class SendGridEmailSender(IConfiguration configuration, ISendGridClient? sendGridClient = null) : IEmailSender
    {
        public async Task EnviarAsync(string destinatarioEmail, string destinatarioNome, string assunto, string texto, CancellationToken cancellationToken)
        {
            var apiKey = configuration["SendGrid:ApiKey"];
            var fromEmail = configuration["SendGrid:FromEmail"];
            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(fromEmail))
                throw new InvalidOperationException("SendGrid não configurado (SendGrid:ApiKey e SendGrid:FromEmail).");

            var client = sendGridClient ?? new SendGridClient(apiKey);
            var mensagem = MailHelper.CreateSingleEmail(
                new EmailAddress(fromEmail, configuration["SendGrid:FromName"]),
                new EmailAddress(destinatarioEmail, destinatarioNome),
                assunto, texto, $"<p>{System.Net.WebUtility.HtmlEncode(texto)}</p>");

            var resposta = await client.SendEmailAsync(mensagem, cancellationToken);
            if (!resposta.IsSuccessStatusCode)
                throw new InvalidOperationException($"SendGrid recusou o envio: {(int)resposta.StatusCode}.");
        }
    }
}
