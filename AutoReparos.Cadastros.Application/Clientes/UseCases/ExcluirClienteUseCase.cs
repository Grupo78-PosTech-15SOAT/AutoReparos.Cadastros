using AutoReparos.Cadastros.Application.Clientes.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Clientes.Repositories;
using AutoReparos.Cadastros.Domain.Shared.Exceptions;

namespace AutoReparos.Cadastros.Application.Clientes.UseCases
{
    public class ExcluirClienteUseCase(IClienteRepository repository) : IExcluirClienteUseCase
    {
        public async Task ExecuteAsync(Guid id)
        {
            var cliente = await repository.GetById(id);
            if (cliente == null)
                throw new NotFoundException("Cliente não encontrado.");

            await repository.Delete(cliente);
        }
    }
}
