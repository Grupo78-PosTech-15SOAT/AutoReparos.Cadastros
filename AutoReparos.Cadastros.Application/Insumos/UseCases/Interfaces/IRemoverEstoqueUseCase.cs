using AutoReparos.Cadastros.Application.Insumos.DTOs.Request;

namespace AutoReparos.Cadastros.Application.Insumos.UseCases.Interfaces
{
    public interface IRemoverEstoqueUseCase
    {
        Task ExecuteAsync(Guid id, AtualizarEstoqueDto dto);
    }
}
