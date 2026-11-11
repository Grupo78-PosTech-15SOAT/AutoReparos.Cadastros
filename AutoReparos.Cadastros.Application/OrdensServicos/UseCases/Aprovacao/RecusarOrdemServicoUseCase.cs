using AutoReparos.Cadastros.Application.OrdensServicos.Services.Interfaces;
using AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Aprovacao.Interfaces;
using AutoReparos.Cadastros.Application.Shared.Interfaces;
using AutoReparos.Cadastros.Domain.Clientes.Repositories;
using AutoReparos.Cadastros.Domain.OrdensServicos.Repositories;
using AutoReparos.Cadastros.Domain.Shared;
using AutoReparos.Cadastros.Domain.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Aprovacao
{
    public class RecusarOrdemServicoUseCase(
        IOrdemServicoRepository repository,
        IAprovacaoTokenService aprovacaoTokenService,
        IClienteRepository clienteRepository,
        INotificacaoService notificacaoService,
        ILogger<RecusarOrdemServicoUseCase> logger) : IRecusarOrdemServicoUseCase
    {
        public async Task ExecuteAsync(string token)
        {
            var ordemServicoId = aprovacaoTokenService.ValidarToken(token);
            var os = await repository.GetById(ordemServicoId)
                ?? throw new NotFoundException(ErrorMessages.OrdemServicoNotFound);

            var statusAnterior = os.Status.ToString();
            os.Recusar();
            await repository.Update(os);

            try
            {
                var cliente = await clienteRepository.GetById(os.ClienteId)
                    ?? throw new NotFoundException(ErrorMessages.ClienteNotFound);
                await notificacaoService.EnviarAtualizacaoStatus(cliente.Email.Endereco, cliente.Nome, os.Id, statusAnterior, os.Status.ToString());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao enviar e-mail de recusa de OS {Id}", os.Id);
            }
        }
    }
}
