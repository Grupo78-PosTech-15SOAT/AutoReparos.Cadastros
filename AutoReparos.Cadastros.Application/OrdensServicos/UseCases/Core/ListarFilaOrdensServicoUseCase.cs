using AutoReparos.Cadastros.Application.OrdensServicos.DTOs.Response;
using AutoReparos.Cadastros.Application.OrdensServicos.Mappers;
using AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Core.Interfaces;
using AutoReparos.Cadastros.Application.Shared;
using AutoReparos.Cadastros.Domain.OrdensServicos.Repositories;

namespace AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Core
{
    public class ListarFilaOrdensServicoUseCase(IOrdemServicoRepository repository) : IListarFilaOrdensServicoUseCase
    {
        public async Task<PagedResult<OrdemServicoDto>> ExecuteAsync(PagedRequest request)
        {
            var (items, total) = await repository.GetFila(request.Skip, request.PageSize);
            return new PagedResult<OrdemServicoDto>(items.Select(OrdemServicoMapper.ToDto), total, request.PageNumber, request.PageSize);
        }
    }
}
