using AutoReparos.Cadastros.Application.Servicos.DTOs.Response;

namespace AutoReparos.Cadastros.Application.Servicos.UseCases.Interfaces
{
    public interface IObterTempoMedioServicosUseCase
    {
        Task<IEnumerable<TempoMedioServicoDto>> ExecuteAsync();
    }
}
