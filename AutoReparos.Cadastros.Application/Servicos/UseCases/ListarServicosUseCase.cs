using AutoReparos.Cadastros.Application.Servicos.DTOs.Request;
using AutoReparos.Cadastros.Application.Servicos.DTOs.Response;
using AutoReparos.Cadastros.Application.Servicos.Mappers;
using AutoReparos.Cadastros.Application.Servicos.UseCases.Interfaces;
using AutoReparos.Cadastros.Application.Shared;
using AutoReparos.Cadastros.Domain.Servicos.Repositories;

namespace AutoReparos.Cadastros.Application.Servicos.UseCases
{
    public class ListarServicosUseCase(IServicoRepository repository) : IListarServicosUseCase
    {
        public async Task<PagedResult<ServicoDto>> ExecuteAsync(ServicoPagedRequest request)
        {
            var (servicos, total) = await repository.GetAll(request.Nome, request.Skip, request.PageSize);
            return new PagedResult<ServicoDto>(servicos.Select(ServicoMapper.ToDto), total, request.PageNumber, request.PageSize);
        }
    }
}
