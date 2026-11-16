using AutoReparos.Cadastros.Application.Veiculos.DTOs.Request;
using AutoReparos.Cadastros.Application.Veiculos.DTOs.Response;

namespace AutoReparos.Cadastros.Application.Veiculos.UseCases.Interfaces
{
    public interface ICriarVeiculoUseCase
    {
        Task<VeiculoDto> ExecuteAsync(VeiculoCreateDto dto);
    }
}
