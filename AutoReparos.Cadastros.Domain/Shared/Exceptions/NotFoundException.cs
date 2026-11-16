namespace AutoReparos.Cadastros.Domain.Shared.Exceptions
{
    public sealed class NotFoundException(string message) : DomainException(message);
}
