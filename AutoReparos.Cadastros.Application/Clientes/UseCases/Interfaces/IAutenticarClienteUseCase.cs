using AutoReparos.Cadastros.Application.Clientes.DTOs.Request;
using AutoReparos.Cadastros.Application.Clientes.DTOs.Response;

namespace AutoReparos.Cadastros.Application.Clientes.UseCases.Interfaces
{
    public interface IAutenticarClienteUseCase
    {
        /// <summary>
        /// Autentica o cliente por CPF e e-mail; devolve <c>null</c> quando não existe, o e-mail não confere ou o cadastro está inativo.
        /// </summary>
        Task<ClienteAutenticadoDto?> ExecuteAsync(AutenticarClienteDto dto);
    }
}
