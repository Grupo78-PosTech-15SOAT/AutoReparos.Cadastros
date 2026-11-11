using AutoReparos.Cadastros.Application.Shared;

namespace AutoReparos.Cadastros.Application.OrdensServicos.DTOs.Request
{
    public record OrdemServicoConsultaPagedRequest : PagedRequest
    {
        public string? Documento { get; init; }
        public string? Placa { get; init; }
    }
}
