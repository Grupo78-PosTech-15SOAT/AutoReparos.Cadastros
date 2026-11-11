using AutoReparos.Cadastros.Application.Usuarios.DTOs.Request;

namespace AutoReparos.Cadastros.Application.Usuarios.UseCases.Interfaces
{
    public interface IAtualizarUsuarioUseCase
    {
        Task ExecuteAsync(Guid id, UsuarioUpdateDto dto);
    }
}
