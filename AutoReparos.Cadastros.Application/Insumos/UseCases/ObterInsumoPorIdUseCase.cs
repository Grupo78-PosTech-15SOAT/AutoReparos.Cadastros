using AutoReparos.Cadastros.Application.Insumos.DTOs.Response;
using AutoReparos.Cadastros.Application.Insumos.Mappers;
using AutoReparos.Cadastros.Application.Insumos.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Insumos.Repositories;

namespace AutoReparos.Cadastros.Application.Insumos.UseCases
{
    public class ObterInsumoPorIdUseCase(IInsumoRepository repository) : IObterInsumoPorIdUseCase
    {
        public async Task<InsumoDto?> ExecuteAsync(Guid id)
        {
            var insumo = await repository.GetById(id);
            return insumo is null ? null : InsumoMapper.ToDto(insumo);
        }
    }
}
