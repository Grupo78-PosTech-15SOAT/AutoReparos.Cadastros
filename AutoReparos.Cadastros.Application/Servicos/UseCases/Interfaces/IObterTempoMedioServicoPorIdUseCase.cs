using AutoReparos.Cadastros.Application.Servicos.DTOs.Response;

namespace AutoReparos.Cadastros.Application.Servicos.UseCases.Interfaces
{
    public interface IObterTempoMedioServicoPorIdUseCase
    {
        Task<TempoMedioServicoDto?> ExecuteAsync(Guid id);
    }
}
