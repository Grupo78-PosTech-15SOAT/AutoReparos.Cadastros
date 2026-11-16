using AutoReparos.Cadastros.Application.Clientes.DTOs.Request;
using AutoReparos.Cadastros.Application.Clientes.DTOs.Response;

namespace AutoReparos.Cadastros.Application.Clientes.UseCases.Interfaces
{
    public interface ICriarClienteUseCase
    {
        Task<ClienteDto> ExecuteAsync(ClienteCreateDto dto);
    }
}
