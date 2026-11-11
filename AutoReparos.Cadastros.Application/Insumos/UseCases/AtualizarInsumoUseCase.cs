using AutoReparos.Cadastros.Application.Insumos.DTOs.Request;
using AutoReparos.Cadastros.Application.Insumos.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Insumos.Repositories;
using AutoReparos.Cadastros.Domain.Shared.Exceptions;

namespace AutoReparos.Cadastros.Application.Insumos.UseCases
{
    public class AtualizarInsumoUseCase(IInsumoRepository repository) : IAtualizarInsumoUseCase
    {
        public async Task ExecuteAsync(Guid id, AtualizarInsumoDto dto)
        {
            var insumo = await repository.GetById(id)
                ?? throw new NotFoundException("Insumo não encontrado.");

            insumo.Atualizar(dto.Nome, dto.Descricao, dto.Valor);
            await repository.Update(insumo);
        }
    }
}
