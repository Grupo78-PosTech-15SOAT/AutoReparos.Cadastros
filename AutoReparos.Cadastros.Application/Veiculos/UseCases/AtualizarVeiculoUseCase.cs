using AutoReparos.Cadastros.Application.Veiculos.DTOs.Request;
using AutoReparos.Cadastros.Application.Veiculos.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Shared.Exceptions;
using AutoReparos.Cadastros.Domain.Veiculos.Repositories;

namespace AutoReparos.Cadastros.Application.Veiculos.UseCases
{
    public class AtualizarVeiculoUseCase(IVeiculoRepository repository) : IAtualizarVeiculoUseCase
    {
        public async Task ExecuteAsync(Guid id, VeiculoUpdateDto dto)
        {
            var veiculo = await repository.GetById(id);

            if (veiculo == null)
                throw new NotFoundException("Veículo não encontrado.");

            veiculo.Atualizar(
                dto.Marca,
                dto.Modelo,
                dto.AnoFabricacao,
                dto.AnoModelo
            );

            await repository.Update(veiculo);
        }
    }
}
