using AutoReparos.Cadastros.Application.OrdensServicos.DTOs.Request;

namespace AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Fluxo.Interfaces
{
    public interface IAdicionarInsumoOrdemServicoUseCase
    {
        Task ExecuteAsync(Guid id, AdicionarInsumoDto dto);
    }
}
