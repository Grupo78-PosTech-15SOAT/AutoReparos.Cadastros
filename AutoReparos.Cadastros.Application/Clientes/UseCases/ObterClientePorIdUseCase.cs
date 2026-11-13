using AutoReparos.Cadastros.Application.Clientes.DTOs.Response;
using AutoReparos.Cadastros.Application.Clientes.Mappers;
using AutoReparos.Cadastros.Application.Clientes.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Clientes.Repositories;
using AutoReparos.Cadastros.Domain.Shared.Exceptions;

namespace AutoReparos.Cadastros.Application.Clientes.UseCases
{
    public class ObterClientePorIdUseCase(IClienteRepository repository) : IObterClientePorIdUseCase
    {
        public async Task<ClienteDto?> ExecuteAsync(Guid id)
        {
            var cliente = await repository.GetById(id);
            if (cliente == null)
                throw new NotFoundException("Cliente não encontrado.");

            return ClienteMapper.ToDto(cliente);
        }
    }
}
