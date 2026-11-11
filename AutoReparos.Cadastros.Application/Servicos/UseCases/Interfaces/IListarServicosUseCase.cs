using AutoReparos.Cadastros.Application.Servicos.DTOs.Request;
using AutoReparos.Cadastros.Application.Servicos.DTOs.Response;
using AutoReparos.Cadastros.Application.Shared;

namespace AutoReparos.Cadastros.Application.Servicos.UseCases.Interfaces
{
    public interface IListarServicosUseCase
    {
        Task<PagedResult<ServicoDto>> ExecuteAsync(ServicoPagedRequest request);
    }
}
