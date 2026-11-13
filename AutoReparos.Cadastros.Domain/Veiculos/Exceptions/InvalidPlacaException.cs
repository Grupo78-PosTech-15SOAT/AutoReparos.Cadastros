using AutoReparos.Cadastros.Domain.Shared.Exceptions;

namespace AutoReparos.Cadastros.Domain.Veiculos.Exceptions
{
    public sealed class InvalidPlacaException(string message) : DomainException(message);
}
