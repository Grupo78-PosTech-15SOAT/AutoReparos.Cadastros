using AutoReparos.Cadastros.Application.Usuarios.DTOs.Request;
using AutoReparos.Cadastros.Application.Usuarios.DTOs.Response;
using AutoReparos.Cadastros.Application.Usuarios.Mappers;
using AutoReparos.Cadastros.Application.Usuarios.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Usuarios.Entities;
using AutoReparos.Cadastros.Domain.Usuarios.Repositories;

namespace AutoReparos.Cadastros.Application.Usuarios.UseCases
{
    public class CriarUsuarioUseCase(IUsuarioRepository usuarioRepository) : ICriarUsuarioUseCase
    {
        public async Task<UsuarioDto> ExecuteAsync(UsuarioCreateDto dto)
        {
            var usuario = new Usuario(dto.NomeCompleto, dto.Email, dto.Tipo);

            await usuarioRepository.CreateAsync(usuario, dto.Password);

            return UsuarioMapper.ToDto(usuario);
        }
    }
}
