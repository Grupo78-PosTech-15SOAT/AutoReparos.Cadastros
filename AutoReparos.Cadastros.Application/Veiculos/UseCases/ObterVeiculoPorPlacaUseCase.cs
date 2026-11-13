using AutoReparos.Cadastros.Application.Veiculos.DTOs.Response;
using AutoReparos.Cadastros.Application.Veiculos.Mappers;
using AutoReparos.Cadastros.Application.Veiculos.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Shared.Exceptions;
using AutoReparos.Cadastros.Domain.Veiculos.Repositories;

namespace AutoReparos.Cadastros.Application.Veiculos.UseCases
{
    public class ObterVeiculoPorPlacaUseCase(IVeiculoRepository repository) : IObterVeiculoPorPlacaUseCase
    {
        public async Task<VeiculoDto?> ExecuteAsync(string placa)
        {
            var veiculo = await repository.GetByPlaca(placa);

            if (veiculo == null)
                throw new NotFoundException("Veículo não encontrado.");

            return VeiculoMapper.ToDto(veiculo);
        }
    }
}
