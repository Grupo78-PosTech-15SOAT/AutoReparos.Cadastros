using AutoReparos.Cadastros.Application.Shared;
using AutoReparos.Cadastros.Domain.OrdensServicos.Enums;

namespace AutoReparos.Cadastros.Application.OrdensServicos.DTOs.Request
{
    public record OrdemServicoPagedRequest : PagedRequest
    {
        public Guid? ClienteId { get; init; }
        public Guid? VeiculoId { get; init; }
        public EStatusOrdemServico? Status { get; init; }
    }
}
