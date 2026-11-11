using AutoReparos.Cadastros.Application.Shared;

namespace AutoReparos.Cadastros.Application.Insumos.DTOs.Request
{
    public record InsumoPagedRequest : PagedRequest
    {
        public string? Nome { get; init; }
    }
}
