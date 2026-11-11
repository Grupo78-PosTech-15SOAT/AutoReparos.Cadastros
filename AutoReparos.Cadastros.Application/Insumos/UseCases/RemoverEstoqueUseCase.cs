using AutoReparos.Cadastros.Application.Insumos.DTOs.Request;
using AutoReparos.Cadastros.Application.Insumos.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Insumos.Repositories;
using AutoReparos.Cadastros.Domain.Shared.Exceptions;

namespace AutoReparos.Cadastros.Application.Insumos.UseCases
{
    public class RemoverEstoqueUseCase(IInsumoRepository repository) : IRemoverEstoqueUseCase
    {
        public async Task ExecuteAsync(Guid id, AtualizarEstoqueDto dto)
        {
            var insumo = await repository.GetById(id)
                ?? throw new NotFoundException("Insumo não encontrado.");

            insumo.RemoverEstoque(dto.Quantidade);
            await repository.Update(insumo);
        }
    }
}
