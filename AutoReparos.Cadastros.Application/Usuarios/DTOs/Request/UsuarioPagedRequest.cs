using AutoReparos.Cadastros.Application.Shared;

namespace AutoReparos.Cadastros.Application.Usuarios.DTOs.Request
{
    public record UsuarioPagedRequest : PagedRequest
    {
        public string? Nome { get; init; }
    }
}
