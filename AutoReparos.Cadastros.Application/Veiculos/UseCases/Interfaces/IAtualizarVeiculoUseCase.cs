using AutoReparos.Cadastros.Application.Veiculos.DTOs.Request;

namespace AutoReparos.Cadastros.Application.Veiculos.UseCases.Interfaces
{
    public interface IAtualizarVeiculoUseCase
    {
        Task ExecuteAsync(Guid id, VeiculoUpdateDto dto);
    }
}
