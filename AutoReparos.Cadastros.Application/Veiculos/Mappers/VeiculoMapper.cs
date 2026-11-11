using AutoReparos.Cadastros.Application.Veiculos.DTOs.Response;
using AutoReparos.Cadastros.Domain.Veiculos.Entities;

namespace AutoReparos.Cadastros.Application.Veiculos.Mappers
{
    public static class VeiculoMapper
    {
        public static VeiculoDto ToDto(Veiculo veiculo) => new(
            veiculo.Id,
            veiculo.ClienteId,
            veiculo.Marca,
            veiculo.Modelo,
            veiculo.AnoFabricacao,
            veiculo.AnoModelo,
            veiculo.Placa.Valor,
            veiculo.Chassi.Valor,
            veiculo.Renavam.Valor
        );
    }
}
