using AutoReparos.Cadastros.Domain.Shared.Exceptions;

namespace AutoReparos.Cadastros.Domain.Clientes.Exceptions
{
    public sealed class InvalidTelefoneException(string message) : DomainException(message);
}
