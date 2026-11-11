using AutoReparos.Cadastros.Domain.Shared.Exceptions;

namespace AutoReparos.Cadastros.Domain.Clientes.Exceptions
{
    public sealed class InvalidClienteException(string message) : DomainException(message);
}