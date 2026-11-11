using AutoReparos.Cadastros.Application.Shared;

namespace AutoReparos.Cadastros.Application.Servicos.DTOs.Request
{
    public record ServicoPagedRequest : PagedRequest
    {
        public string? Nome { get; init; }
    }
}
