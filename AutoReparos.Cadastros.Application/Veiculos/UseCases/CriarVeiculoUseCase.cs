using AutoReparos.Cadastros.Application.Veiculos.DTOs.Request;
using AutoReparos.Cadastros.Application.Veiculos.DTOs.Response;
using AutoReparos.Cadastros.Application.Veiculos.Mappers;
using AutoReparos.Cadastros.Application.Veiculos.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Clientes.Repositories;
using AutoReparos.Cadastros.Domain.Shared.Exceptions;
using AutoReparos.Cadastros.Domain.Veiculos.Entities;
using AutoReparos.Cadastros.Domain.Veiculos.Repositories;
using AutoReparos.Cadastros.Domain.Veiculos.ValueObjects;

namespace AutoReparos.Cadastros.Application.Veiculos.UseCases
{
    public class CriarVeiculoUseCase(IVeiculoRepository repository, IClienteRepository clienteRepository) : ICriarVeiculoUseCase
    {
        public async Task<VeiculoDto> ExecuteAsync(VeiculoCreateDto dto)
        {
            var cliente = await clienteRepository.GetById(dto.ClienteId);
            if (cliente == null)
                throw new NotFoundException("Cliente não encontrado.");

            var veiculo = new Veiculo(
                dto.ClienteId,
                dto.Marca,
                dto.Modelo,
                dto.AnoFabricacao,
                dto.AnoModelo,
                new Placa(dto.Placa),
                new Chassi(dto.Chassi),
                new Renavam(dto.Renavam)
            );

            await repository.Create(veiculo);

            return VeiculoMapper.ToDto(veiculo);
        }
    }
}
