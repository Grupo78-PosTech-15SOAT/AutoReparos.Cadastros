using AutoReparos.Cadastros.Application.OrdensServicos.DTOs.Request;
using AutoReparos.Cadastros.Application.OrdensServicos.DTOs.Response;

namespace AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Core.Interfaces
{
    public interface ICriarOrdemServicoUseCase
    {
        Task<OrdemServicoDto> ExecuteAsync(CriarOrdemServicoDto dto);
    }
}
