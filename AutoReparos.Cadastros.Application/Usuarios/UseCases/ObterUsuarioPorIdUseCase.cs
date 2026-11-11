using AutoReparos.Cadastros.Application.Usuarios.DTOs.Response;
using AutoReparos.Cadastros.Application.Usuarios.Mappers;
using AutoReparos.Cadastros.Application.Usuarios.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Shared.Exceptions;
using AutoReparos.Cadastros.Domain.Usuarios.Repositories;

namespace AutoReparos.Cadastros.Application.Usuarios.UseCases
{
    public class ObterUsuarioPorIdUseCase(IUsuarioRepository usuarioRepository) : IObterUsuarioPorIdUseCase
    {
        public async Task<UsuarioDto?> ExecuteAsync(Guid id)
        {
            var usuario = await usuarioRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("Usuário não encontrado no sistema.");

            return UsuarioMapper.ToDto(usuario);
        }
    }
}
