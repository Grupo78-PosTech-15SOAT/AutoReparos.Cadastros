using AutoReparos.Cadastros.Application.Usuarios.DTOs.Response;

namespace AutoReparos.Cadastros.Application.Usuarios.UseCases.Interfaces
{
    public interface IObterUsuarioPorIdUseCase
    {
        Task<UsuarioDto?> ExecuteAsync(Guid id);
    }
}
