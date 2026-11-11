using AutoReparos.Cadastros.Application.Servicos.DTOs.Request;

namespace AutoReparos.Cadastros.Application.Servicos.UseCases.Interfaces
{
    public interface IAtualizarServicoUseCase
    {
        Task ExecuteAsync(Guid id, AtualizarServicoDto dto);
    }
}
