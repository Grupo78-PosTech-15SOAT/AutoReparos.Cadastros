using AutoReparos.Cadastros.Domain.Shared.Exceptions;

namespace AutoReparos.Cadastros.Domain.Veiculos.Exceptions
{
    public sealed class DuplicatedPlacaException() : DomainException("Já existe um veículo com essa placa");
}
