using AutoReparos.Cadastros.Application.Veiculos.DTOs.Response;
using AutoReparos.Cadastros.Application.Veiculos.Mappers;
using AutoReparos.Cadastros.Application.Veiculos.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Shared.Exceptions;
using AutoReparos.Cadastros.Domain.Veiculos.Repositories;

namespace AutoReparos.Cadastros.Application.Veiculos.UseCases
{
    public class ObterVeiculoPorIdUseCase(IVeiculoRepository repository) : IObterVeiculoPorIdUseCase
    {
        public async Task<VeiculoDto?> ExecuteAsync(Guid id)
        {
            var veiculo = await repository.GetById(id);

            if (veiculo == null)
                throw new NotFoundException("Veículo não encontrado.");

            return VeiculoMapper.ToDto(veiculo);
        }
    }
}
