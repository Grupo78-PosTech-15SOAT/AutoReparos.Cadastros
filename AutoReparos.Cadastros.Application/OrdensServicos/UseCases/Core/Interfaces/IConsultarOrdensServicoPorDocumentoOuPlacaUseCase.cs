using AutoReparos.Cadastros.Application.OrdensServicos.DTOs.Request;
using AutoReparos.Cadastros.Application.OrdensServicos.DTOs.Response;
using AutoReparos.Cadastros.Application.Shared;

namespace AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Core.Interfaces
{
    public interface IConsultarOrdensServicoPorDocumentoOuPlacaUseCase
    {
        Task<PagedResult<OrdemServicoPublicoDto>> ExecuteAsync(OrdemServicoConsultaPagedRequest request);
    }
}
