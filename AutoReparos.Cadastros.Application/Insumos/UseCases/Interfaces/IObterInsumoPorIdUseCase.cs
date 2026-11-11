using AutoReparos.Cadastros.Application.Insumos.DTOs.Response;

namespace AutoReparos.Cadastros.Application.Insumos.UseCases.Interfaces
{
    public interface IObterInsumoPorIdUseCase
    {
        Task<InsumoDto?> ExecuteAsync(Guid id);
    }
}
