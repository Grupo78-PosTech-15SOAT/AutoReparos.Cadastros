using AutoReparos.Cadastros.Application.Usuarios.DTOs.Request;
using AutoReparos.Cadastros.Application.Usuarios.DTOs.Response;

namespace AutoReparos.Cadastros.Application.Usuarios.UseCases.Interfaces
{
    public interface ICriarUsuarioUseCase
    {
        Task<UsuarioDto> ExecuteAsync(UsuarioCreateDto dto);
    }
}
