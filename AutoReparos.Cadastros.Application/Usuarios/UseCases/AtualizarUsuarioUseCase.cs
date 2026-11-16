using AutoReparos.Cadastros.Application.Usuarios.DTOs.Request;
using AutoReparos.Cadastros.Application.Usuarios.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Shared.Exceptions;
using AutoReparos.Cadastros.Domain.Usuarios.Repositories;

namespace AutoReparos.Cadastros.Application.Usuarios.UseCases
{
    public class AtualizarUsuarioUseCase(IUsuarioRepository usuarioRepository) : IAtualizarUsuarioUseCase
    {
        public async Task ExecuteAsync(Guid id, UsuarioUpdateDto dto)
        {
            var usuario = await usuarioRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("Usuário não encontrado para atualização.");

            usuario.Atualizar(dto.NomeCompleto, dto.Tipo);

            await usuarioRepository.UpdateAsync(usuario);
        }
    }
}
