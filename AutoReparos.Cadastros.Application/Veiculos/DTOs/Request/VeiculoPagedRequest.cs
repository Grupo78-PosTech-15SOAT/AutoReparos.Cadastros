using AutoReparos.Cadastros.Application.Shared;

namespace AutoReparos.Cadastros.Application.Veiculos.DTOs.Request
{
    public record VeiculoPagedRequest : PagedRequest
    {
        public Guid? ClienteId { get; init; }
    }
}
