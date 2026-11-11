using AutoReparos.Cadastros.Application.Shared;
using AutoReparos.Cadastros.Application.Usuarios.DTOs.Request;
using AutoReparos.Cadastros.Application.Usuarios.DTOs.Response;
using AutoReparos.Cadastros.Application.Usuarios.Mappers;
using AutoReparos.Cadastros.Application.Usuarios.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Usuarios.Repositories;

namespace AutoReparos.Cadastros.Application.Usuarios.UseCases
{
    public class ListarUsuariosUseCase(IUsuarioRepository usuarioRepository) : IListarUsuariosUseCase
    {
        public async Task<PagedResult<UsuarioDto>> ExecuteAsync(UsuarioPagedRequest request)
        {
            var (usuarios, total) = await usuarioRepository.GetAllAsync(
                request.Nome,
                request.Skip,
                request.PageSize);

            var items = usuarios.Select(UsuarioMapper.ToDto);
            return new PagedResult<UsuarioDto>(items, total, request.PageNumber, request.PageSize);
        }
    }
}
