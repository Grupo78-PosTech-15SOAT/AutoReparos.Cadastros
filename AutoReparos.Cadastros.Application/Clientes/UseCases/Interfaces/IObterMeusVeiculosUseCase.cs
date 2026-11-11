using AutoReparos.Cadastros.Application.Veiculos.DTOs.Response;

namespace AutoReparos.Cadastros.Application.Clientes.UseCases.Interfaces;

public interface IObterMeusVeiculosUseCase
{
    Task<IEnumerable<VeiculoDto>> ExecuteAsync(Guid clienteId, CancellationToken cancellationToken = default);
}
