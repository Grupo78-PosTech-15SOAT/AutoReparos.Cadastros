using AutoReparos.Cadastros.Application.Clientes.DTOs.Request;
using AutoReparos.Cadastros.Application.Clientes.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Clientes.Repositories;
using AutoReparos.Cadastros.Domain.Shared.Exceptions;

namespace AutoReparos.Cadastros.Application.Clientes.UseCases
{
    public class AtualizarClienteUseCase(IClienteRepository repository) : IAtualizarClienteUseCase
    {
        public async Task ExecuteAsync(Guid id, ClienteUpdateDto dto)
        {
            var cliente = await repository.GetById(id);
            if (cliente == null)
                throw new NotFoundException("Cliente não encontrado.");

            cliente.Atualizar(dto.Nome, dto.Email, dto.Telefone);
            await repository.Update(cliente);
        }
    }
}
