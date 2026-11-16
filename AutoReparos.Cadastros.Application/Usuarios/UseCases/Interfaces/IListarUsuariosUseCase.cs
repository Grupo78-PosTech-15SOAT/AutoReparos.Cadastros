using AutoReparos.Cadastros.Application.Shared;
using AutoReparos.Cadastros.Application.Usuarios.DTOs.Request;
using AutoReparos.Cadastros.Application.Usuarios.DTOs.Response;

namespace AutoReparos.Cadastros.Application.Usuarios.UseCases.Interfaces
{
    public interface IListarUsuariosUseCase
    {
        Task<PagedResult<UsuarioDto>> ExecuteAsync(UsuarioPagedRequest request);
    }
}
