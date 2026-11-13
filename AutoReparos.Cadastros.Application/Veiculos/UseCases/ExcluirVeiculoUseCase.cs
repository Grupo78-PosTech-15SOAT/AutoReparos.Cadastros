using AutoReparos.Cadastros.Application.Veiculos.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Shared.Exceptions;
using AutoReparos.Cadastros.Domain.Veiculos.Repositories;

namespace AutoReparos.Cadastros.Application.Veiculos.UseCases
{
    public class ExcluirVeiculoUseCase(IVeiculoRepository repository) : IExcluirVeiculoUseCase
    {
        public async Task ExecuteAsync(Guid id)
        {
            var veiculo = await repository.GetById(id);

            if (veiculo == null)
                throw new NotFoundException("Veículo não encontrado.");

            await repository.Delete(veiculo);
        }
    }
}
