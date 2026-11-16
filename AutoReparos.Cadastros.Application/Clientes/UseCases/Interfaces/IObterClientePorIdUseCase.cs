using AutoReparos.Cadastros.Application.Clientes.DTOs.Response;

namespace AutoReparos.Cadastros.Application.Clientes.UseCases.Interfaces
{
    public interface IObterClientePorIdUseCase
    {
        Task<ClienteDto?> ExecuteAsync(Guid id);
    }
}
