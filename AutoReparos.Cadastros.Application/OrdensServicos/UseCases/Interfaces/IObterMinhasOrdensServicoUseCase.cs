using AutoReparos.Cadastros.Application.OrdensServicos.DTOs.Response;

namespace AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Interfaces;

public interface IObterMinhasOrdensServicoUseCase
{
    Task<IEnumerable<MinhaOrdemServicoDto>> ExecuteAsync(Guid clienteId, string? placa = null, CancellationToken cancellationToken = default);
}
