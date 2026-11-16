using AutoReparos.Cadastros.Domain.Shared.Exceptions;

namespace AutoReparos.Cadastros.Domain.Veiculos.Exceptions
{
    public sealed class InvalidVeiculoException(string message) : DomainException(message);
}