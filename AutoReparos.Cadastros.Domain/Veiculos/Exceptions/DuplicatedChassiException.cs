using AutoReparos.Cadastros.Domain.Shared.Exceptions;

namespace AutoReparos.Cadastros.Domain.Veiculos.Exceptions
{
    public sealed class DuplicatedChassiException() : DomainException("Já existe um veículo com esse chassi");
}