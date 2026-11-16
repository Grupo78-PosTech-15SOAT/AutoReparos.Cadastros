using AutoReparos.Cadastros.Application.Clientes.DTOs.Request;
using AutoReparos.Cadastros.Application.Clientes.DTOs.Response;

namespace AutoReparos.Cadastros.Application.Clientes.UseCases.Interfaces
{
    public enum StatusAutenticacaoCliente
    {
        Autenticado,
        NaoEncontrado,
        Inativo
    }

    public record ResultadoAutenticacaoCliente(StatusAutenticacaoCliente Status, ClienteAutenticadoDto? Cliente = null);

    public interface IAutenticarClienteUseCase
    {
        /// <summary>
        /// Autentica o cliente por CPF e e-mail. <c>NaoEncontrado</c> cobre CPF inexistente e e-mail divergente (sem distinguir, para não revelar cadastros).
        /// </summary>
        Task<ResultadoAutenticacaoCliente> ExecuteAsync(AutenticarClienteDto dto);
    }
}
