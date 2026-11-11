using AutoReparos.Cadastros.Application.Insumos.DTOs.Request;
using AutoReparos.Cadastros.Application.Insumos.DTOs.Response;

namespace AutoReparos.Cadastros.Application.Insumos.UseCases.Interfaces
{
    public interface ICriarInsumoUseCase
    {
        Task<InsumoDto> ExecuteAsync(CriarInsumoDto dto);
    }
}
