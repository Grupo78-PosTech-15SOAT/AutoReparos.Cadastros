using AutoReparos.Cadastros.Application.OrdensServicos.DTOs.Request;
using AutoReparos.Cadastros.Application.OrdensServicos.DTOs.Response;
using AutoReparos.Cadastros.Application.OrdensServicos.Mappers;
using AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Core.Interfaces;
using AutoReparos.Cadastros.Application.Shared;
using AutoReparos.Cadastros.Domain.OrdensServicos.Repositories;

namespace AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Core
{
    public class ConsultarOrdensServicoPorDocumentoOuPlacaUseCase(IOrdemServicoRepository repository) : IConsultarOrdensServicoPorDocumentoOuPlacaUseCase
    {
        public async Task<PagedResult<OrdemServicoPublicoDto>> ExecuteAsync(OrdemServicoConsultaPagedRequest request)
        {
            var (items, total) = await repository.GetByDocumentoOuPlaca(request.Documento, request.Placa, request.Skip, request.PageSize);
            return new PagedResult<OrdemServicoPublicoDto>(items.Select(OrdemServicoMapper.ToPublicDto), total, request.PageNumber, request.PageSize);
        }
    }
}
