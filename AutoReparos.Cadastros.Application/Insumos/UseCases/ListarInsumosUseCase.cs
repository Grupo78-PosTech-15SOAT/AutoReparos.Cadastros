using AutoReparos.Cadastros.Application.Insumos.DTOs.Request;
using AutoReparos.Cadastros.Application.Insumos.DTOs.Response;
using AutoReparos.Cadastros.Application.Insumos.Mappers;
using AutoReparos.Cadastros.Application.Insumos.UseCases.Interfaces;
using AutoReparos.Cadastros.Application.Shared;
using AutoReparos.Cadastros.Domain.Insumos.Repositories;

namespace AutoReparos.Cadastros.Application.Insumos.UseCases
{
    public class ListarInsumosUseCase(IInsumoRepository repository) : IListarInsumosUseCase
    {
        public async Task<PagedResult<InsumoDto>> ExecuteAsync(InsumoPagedRequest request)
        {
            var (items, total) = await repository.GetAll(request.Nome, request.Skip, request.PageSize);
            return new PagedResult<InsumoDto>(items.Select(InsumoMapper.ToDto), total, request.PageNumber, request.PageSize);
        }
    }
}
