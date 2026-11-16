using AutoReparos.Cadastros.Domain.Shared.Exceptions;

namespace AutoReparos.Cadastros.Domain.Veiculos.Exceptions
{
    public sealed class InvalidRenavamException(string message) : DomainException(message);
}
