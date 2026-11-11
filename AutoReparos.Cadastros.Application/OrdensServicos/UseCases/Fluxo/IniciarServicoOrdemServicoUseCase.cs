using AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Fluxo.Interfaces;
using AutoReparos.Cadastros.Domain.OrdensServicos.Repositories;
using AutoReparos.Cadastros.Domain.Shared;
using AutoReparos.Cadastros.Domain.Shared.Exceptions;

namespace AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Fluxo
{
    public class IniciarServicoOrdemServicoUseCase(IOrdemServicoRepository repository) : IIniciarServicoOrdemServicoUseCase
    {
        public async Task ExecuteAsync(Guid ordemServicoId, Guid ordemServicoServicoId)
        {
            var os = await repository.GetById(ordemServicoId)
                ?? throw new NotFoundException(ErrorMessages.OrdemServicoNotFound);

            os.IniciarServico(ordemServicoServicoId);
            await repository.Update(os);
        }
    }
}
