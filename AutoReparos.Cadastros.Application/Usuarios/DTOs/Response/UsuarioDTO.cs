using AutoReparos.Cadastros.Domain.Usuarios.Enums;

namespace AutoReparos.Cadastros.Application.Usuarios.DTOs.Response
{
    public record UsuarioDto(
        Guid Id,
        string NomeCompleto,
        string Email,
        ETipoUsuario Tipo,
        DateTime CriadoEm
    );
}
