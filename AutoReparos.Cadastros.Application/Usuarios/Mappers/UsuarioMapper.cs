using AutoReparos.Cadastros.Application.Usuarios.DTOs.Response;
using AutoReparos.Cadastros.Domain.Usuarios.Entities;

namespace AutoReparos.Cadastros.Application.Usuarios.Mappers
{
    public static class UsuarioMapper
    {
        public static UsuarioDto ToDto(Usuario usuario) => new(
            usuario.Id,
            usuario.NomeCompleto,
            usuario.Email.Endereco,
            usuario.Tipo,
            usuario.CriadoEm
        );
    }
}
