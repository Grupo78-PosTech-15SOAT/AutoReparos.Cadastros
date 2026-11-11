using AutoReparos.Cadastros.Application.Insumos.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Insumos.Repositories;
using AutoReparos.Cadastros.Domain.Shared.Exceptions;

namespace AutoReparos.Cadastros.Application.Insumos.UseCases
{
    public class ExcluirInsumoUseCase(IInsumoRepository repository) : IExcluirInsumoUseCase
    {
        public async Task ExecuteAsync(Guid id)
        {
            var insumo = await repository.GetById(id)
                ?? throw new NotFoundException("Insumo não encontrado.");

            await repository.Delete(insumo);
        }
    }
}
