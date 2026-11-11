using AutoReparos.Cadastros.Application.OrdensServicos.DTOs.Response;

namespace AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Core.Interfaces
{
    public interface IObterOrdemServicoPublicaPorIdUseCase
    {
        Task<OrdemServicoPublicoDetalheDto?> ExecuteAsync(Guid id);
    }
}
