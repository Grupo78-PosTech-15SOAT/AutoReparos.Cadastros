using AutoReparos.Cadastros.Application.OrdensServicos.DTOs.Request;
using AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Fluxo.Interfaces;
using AutoReparos.Cadastros.Domain.OrdensServicos.Entities;
using AutoReparos.Cadastros.Domain.OrdensServicos.Repositories;
using AutoReparos.Cadastros.Domain.Shared;
using AutoReparos.Cadastros.Domain.Shared.Exceptions;

namespace AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Fluxo
{
    public class AdicionarServicoOrdemServicoUseCase(IOrdemServicoRepository repository) : IAdicionarServicoOrdemServicoUseCase
    {
        public async Task ExecuteAsync(Guid id, AdicionarServicoDto dto)
        {
            var os = await repository.GetById(id)
                ?? throw new NotFoundException(ErrorMessages.OrdemServicoNotFound);

            var item = new OrdemServicoServico(id, dto.ServicoId, dto.ValorCobrado);
            os.AdicionarServico(item);
            await repository.Update(os);
        }
    }
}
