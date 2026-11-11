using AutoReparos.Cadastros.Application.OrdensServicos.DTOs.Request;
using AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Fluxo.Interfaces;
using AutoReparos.Cadastros.Domain.Insumos.Repositories;
using AutoReparos.Cadastros.Domain.OrdensServicos.Entities;
using AutoReparos.Cadastros.Domain.OrdensServicos.Enums;
using AutoReparos.Cadastros.Domain.OrdensServicos.Repositories;
using AutoReparos.Cadastros.Domain.Shared;
using AutoReparos.Cadastros.Domain.Shared.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Fluxo
{
    public class AdicionarInsumoOrdemServicoUseCase(IOrdemServicoRepository repository, IInsumoRepository insumoRepository) : IAdicionarInsumoOrdemServicoUseCase
    {
        public async Task ExecuteAsync(Guid id, AdicionarInsumoDto dto)
        {
            var os = await repository.GetById(id)
                ?? throw new NotFoundException(ErrorMessages.OrdemServicoNotFound);

            decimal valorUnitario;

            if (dto.Origem == EOrigemInsumo.Estoque)
            {
                var insumo = await insumoRepository.GetById(dto.InsumoId!.Value)
                    ?? throw new NotFoundException(ErrorMessages.InsumoNotFound);

                insumo.RemoverEstoque(dto.Quantidade);
                await insumoRepository.Update(insumo);

                valorUnitario = dto.ValorUnitario ?? insumo.Valor;
            }
            else
            {
                if (dto.ValorUnitario == null)
                    throw new ValidationException("Valor unitário é obrigatório para insumos de compra específica.");

                valorUnitario = dto.ValorUnitario.Value;
            }

            var item = new OrdemServicoInsumo(id, dto.InsumoId, dto.Descricao, valorUnitario, dto.Quantidade, dto.Origem);
            os.AdicionarInsumo(item);
            await repository.Update(os);
        }
    }
}
