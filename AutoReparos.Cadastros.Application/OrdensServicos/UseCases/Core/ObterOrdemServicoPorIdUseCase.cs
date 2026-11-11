using System;
using System.Threading.Tasks;
using AutoReparos.Cadastros.Application.OrdensServicos.DTOs.Response;
using AutoReparos.Cadastros.Application.OrdensServicos.Mappers;
using AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Core.Interfaces;
using AutoReparos.Cadastros.Domain.OrdensServicos.Repositories;
using AutoReparos.Cadastros.Domain.Usuarios.Repositories;

namespace AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Core
{
    public class ObterOrdemServicoPorIdUseCase(
        IOrdemServicoRepository repository,
        IUsuarioRepository usuarioRepository) : IObterOrdemServicoPorIdUseCase
    {
        public async Task<OrdemServicoDetalheDto?> ExecuteAsync(Guid id)
        {
            var os = await repository.GetById(id);
            if (os is null) return null;

            string? responsavelNome = null;
            if (!string.IsNullOrEmpty(os.ResponsavelId) && Guid.TryParse(os.ResponsavelId, out var responsavelGuid))
            {
                var usuario = await usuarioRepository.GetByIdAsync(responsavelGuid);
                responsavelNome = usuario?.NomeCompleto;
            }

            return OrdemServicoMapper.ToDetalheDto(os, responsavelNome);
        }
    }
}
