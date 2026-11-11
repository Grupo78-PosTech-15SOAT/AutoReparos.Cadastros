using AutoReparos.Cadastros.Domain.Shared.Exceptions;

namespace AutoReparos.Cadastros.Domain.Insumos.Exceptions
{
    public class InvalidInsumoException(string message) : DomainException(message);
}
