using AutoReparos.Cadastros.Application.Shared;
using AutoReparos.Cadastros.Application.Veiculos.DTOs.Request;
using AutoReparos.Cadastros.Application.Veiculos.DTOs.Response;

namespace AutoReparos.Cadastros.Application.Veiculos.UseCases.Interfaces
{
    public interface IListarVeiculosUseCase
    {
        Task<PagedResult<VeiculoDto>> ExecuteAsync(VeiculoPagedRequest request);
    }
}
