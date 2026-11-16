using AutoReparos.Cadastros.Domain.Shared.Exceptions;

namespace AutoReparos.Cadastros.Domain.Usuarios.Exceptions
{
    public sealed class InvalidUsuarioException(string message) : DomainException(message);
}
