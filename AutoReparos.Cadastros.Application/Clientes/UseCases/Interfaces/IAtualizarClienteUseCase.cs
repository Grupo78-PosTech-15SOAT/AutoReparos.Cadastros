using AutoReparos.Cadastros.Application.Clientes.DTOs.Request;

namespace AutoReparos.Cadastros.Application.Clientes.UseCases.Interfaces
{
    public interface IAtualizarClienteUseCase
    {
        Task ExecuteAsync(Guid id, ClienteUpdateDto dto);
    }
}
