using AutoReparos.Cadastros.Application.Insumos.DTOs.Request;

namespace AutoReparos.Cadastros.Application.Insumos.UseCases.Interfaces
{
    public interface IAtualizarInsumoUseCase
    {
        Task ExecuteAsync(Guid id, AtualizarInsumoDto dto);
    }
}
