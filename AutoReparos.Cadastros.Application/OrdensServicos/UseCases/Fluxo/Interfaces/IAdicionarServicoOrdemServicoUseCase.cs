using AutoReparos.Cadastros.Application.OrdensServicos.DTOs.Request;

namespace AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Fluxo.Interfaces
{
    public interface IAdicionarServicoOrdemServicoUseCase
    {
        Task ExecuteAsync(Guid id, AdicionarServicoDto dto);
    }
}
