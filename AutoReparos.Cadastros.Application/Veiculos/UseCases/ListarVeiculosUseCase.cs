using AutoReparos.Cadastros.Application.Shared;
using AutoReparos.Cadastros.Application.Veiculos.DTOs.Request;
using AutoReparos.Cadastros.Application.Veiculos.DTOs.Response;
using AutoReparos.Cadastros.Application.Veiculos.Mappers;
using AutoReparos.Cadastros.Application.Veiculos.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Veiculos.Repositories;

namespace AutoReparos.Cadastros.Application.Veiculos.UseCases
{
    public class ListarVeiculosUseCase(IVeiculoRepository repository) : IListarVeiculosUseCase
    {
        public async Task<PagedResult<VeiculoDto>> ExecuteAsync(VeiculoPagedRequest request)
        {
            var (items, total) = await repository.GetAll(request.ClienteId, request.Skip, request.PageSize);
            return new PagedResult<VeiculoDto>(items.Select(VeiculoMapper.ToDto), total, request.PageNumber, request.PageSize);
        }
    }
}
