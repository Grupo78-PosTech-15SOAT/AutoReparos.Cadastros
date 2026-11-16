using AutoReparos.Cadastros.Application.Clientes.DTOs.Request;
using AutoReparos.Cadastros.Application.Clientes.DTOs.Response;
using AutoReparos.Cadastros.Application.Clientes.Mappers;
using AutoReparos.Cadastros.Application.Clientes.UseCases.Interfaces;
using AutoReparos.Cadastros.Application.Shared;
using AutoReparos.Cadastros.Domain.Clientes.Repositories;

namespace AutoReparos.Cadastros.Application.Clientes.UseCases
{
    public class ListarClientesUseCase(IClienteRepository repository) : IListarClientesUseCase
    {
        public async Task<PagedResult<ClienteDto>> ExecuteAsync(ClientePagedRequest request)
        {
            var (clientes, total) = await repository.GetAll(request.Nome, request.Skip, request.PageSize);
            var items = clientes.Select(ClienteMapper.ToDto);
            return new PagedResult<ClienteDto>(items, total, request.PageNumber, request.PageSize);
        }
    }
}
