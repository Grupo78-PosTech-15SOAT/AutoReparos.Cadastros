using AutoReparos.Cadastros.Application.Shared;

namespace AutoReparos.Cadastros.Application.Clientes.DTOs.Request
{
    public record ClientePagedRequest : PagedRequest
    {
        public string? Nome { get; init; }
    }
}
