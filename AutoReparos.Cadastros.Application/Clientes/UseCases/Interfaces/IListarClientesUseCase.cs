using AutoReparos.Cadastros.Application.Clientes.DTOs.Request;
using AutoReparos.Cadastros.Application.Clientes.DTOs.Response;
using AutoReparos.Cadastros.Application.Shared;

namespace AutoReparos.Cadastros.Application.Clientes.UseCases.Interfaces
{
    public interface IListarClientesUseCase
    {
        Task<PagedResult<ClienteDto>> ExecuteAsync(ClientePagedRequest request);
    }
}
