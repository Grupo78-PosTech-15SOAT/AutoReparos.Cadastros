using AutoReparos.Cadastros.Application.Veiculos.DTOs.Response;

namespace AutoReparos.Cadastros.Application.Veiculos.UseCases.Interfaces
{
    public interface IObterVeiculoPorPlacaUseCase
    {
        Task<VeiculoDto?> ExecuteAsync(string placa);
    }
}
