namespace AutoReparos.Cadastros.Application.OrdensServicos.DTOs.Response
{
    public record OrdemServicoDto(
        Guid Id,
        Guid ClienteId,
        Guid VeiculoId,
        string Status,
        string? Observacao,
        decimal ValorTotal,
        DateTime CriadoEm,
        DateTime? IniciadoEm,
        DateTime? FinalizadoEm,
        DateTime? EntregueEm
    );
}
