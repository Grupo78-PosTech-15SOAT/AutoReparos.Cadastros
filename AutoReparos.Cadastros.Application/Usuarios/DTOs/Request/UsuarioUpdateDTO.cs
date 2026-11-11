using AutoReparos.Cadastros.Domain.Usuarios.Enums;

namespace AutoReparos.Cadastros.Application.Usuarios.DTOs.Request
{
    public record UsuarioUpdateDto(
        string NomeCompleto,
        ETipoUsuario Tipo
    );
}
