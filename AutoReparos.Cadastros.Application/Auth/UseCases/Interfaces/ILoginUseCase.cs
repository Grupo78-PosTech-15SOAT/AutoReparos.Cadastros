using AutoReparos.Cadastros.Application.Auth.DTOs.Request;
using AutoReparos.Cadastros.Application.Auth.DTOs.Response;

namespace AutoReparos.Cadastros.Application.Auth.UseCases.Interfaces
{
    public interface ILoginUseCase
    {
        Task<LoginResponseDto?> ExecuteAsync(LoginRequestDto loginRequest);
    }
}
