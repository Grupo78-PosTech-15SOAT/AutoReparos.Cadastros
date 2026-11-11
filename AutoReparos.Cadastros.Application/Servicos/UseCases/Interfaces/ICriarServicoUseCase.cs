using AutoReparos.Cadastros.Application.Servicos.DTOs.Request;
using AutoReparos.Cadastros.Application.Servicos.DTOs.Response;

namespace AutoReparos.Cadastros.Application.Servicos.UseCases.Interfaces
{
    public interface ICriarServicoUseCase
    {
        Task<ServicoDto> ExecuteAsync(CriarServicoDto dto);
    }
}
