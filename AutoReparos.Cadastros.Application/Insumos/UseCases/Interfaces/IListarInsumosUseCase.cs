using AutoReparos.Cadastros.Application.Insumos.DTOs.Request;
using AutoReparos.Cadastros.Application.Insumos.DTOs.Response;
using AutoReparos.Cadastros.Application.Shared;

namespace AutoReparos.Cadastros.Application.Insumos.UseCases.Interfaces
{
    public interface IListarInsumosUseCase
    {
        Task<PagedResult<InsumoDto>> ExecuteAsync(InsumoPagedRequest request);
    }
}
