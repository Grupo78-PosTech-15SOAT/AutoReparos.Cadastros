using AutoReparos.Cadastros.Application.OrdensServicos.DTOs.Response;
using AutoReparos.Cadastros.Application.Shared;

namespace AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Core.Interfaces
{
    public interface IListarFilaOrdensServicoUseCase
    {
        Task<PagedResult<OrdemServicoDto>> ExecuteAsync(PagedRequest request);
    }
}
