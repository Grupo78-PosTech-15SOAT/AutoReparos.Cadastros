using AutoReparos.Cadastros.Application.Servicos.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Servicos.Repositories;
using AutoReparos.Cadastros.Domain.Shared.Exceptions;

namespace AutoReparos.Cadastros.Application.Servicos.UseCases
{
    public class ExcluirServicoUseCase(IServicoRepository repository) : IExcluirServicoUseCase
    {
        public async Task ExecuteAsync(Guid id)
        {
            var servico = await repository.GetById(id)
                ?? throw new NotFoundException("Serviço não encontrado.");

            await repository.Delete(servico);
        }
    }
}
