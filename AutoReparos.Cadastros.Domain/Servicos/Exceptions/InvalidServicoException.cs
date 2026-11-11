using AutoReparos.Cadastros.Domain.Shared.Exceptions;

namespace AutoReparos.Cadastros.Domain.Servicos.Exceptions
{
    public class InvalidServicoException(string message) : DomainException(message);
}
