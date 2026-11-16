using AutoReparos.Cadastros.Domain.Shared.Exceptions;

namespace AutoReparos.Cadastros.Domain.Veiculos.Exceptions
{
    public sealed class DuplicatedRenavamException() : DomainException("Já existe um veículo com esse RENAVAM");
}
