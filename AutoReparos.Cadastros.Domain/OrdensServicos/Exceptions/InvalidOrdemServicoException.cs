using AutoReparos.Cadastros.Domain.Shared.Exceptions;

namespace AutoReparos.Cadastros.Domain.OrdensServicos.Exceptions
{
    public class InvalidOrdemServicoException(string message) : DomainException(message);
}
