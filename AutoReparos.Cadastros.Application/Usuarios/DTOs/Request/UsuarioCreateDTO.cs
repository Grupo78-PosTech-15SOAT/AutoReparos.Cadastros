using AutoReparos.Cadastros.Domain.Usuarios.Enums;

namespace AutoReparos.Cadastros.Application.Usuarios.DTOs.Request
{
    public record UsuarioCreateDto(
        string NomeCompleto,
        string Email,
        string Password,
        ETipoUsuario Tipo
    );
}
