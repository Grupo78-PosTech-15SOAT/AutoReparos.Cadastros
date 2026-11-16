using AutoReparos.Cadastros.Application.Veiculos.DTOs.Response;

namespace AutoReparos.Cadastros.Application.Veiculos.UseCases.Interfaces
{
    public interface IObterVeiculoPorIdUseCase
    {
        Task<VeiculoDto?> ExecuteAsync(Guid id);
    }
}
