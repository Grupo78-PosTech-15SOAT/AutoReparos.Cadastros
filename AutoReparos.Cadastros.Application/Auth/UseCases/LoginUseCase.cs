using AutoReparos.Cadastros.Application.Auth.DTOs.Request;
using AutoReparos.Cadastros.Application.Auth.DTOs.Response;
using AutoReparos.Cadastros.Application.Auth.Services.Interfaces;
using AutoReparos.Cadastros.Application.Auth.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Usuarios.Repositories;

namespace AutoReparos.Cadastros.Application.Auth.UseCases
{
    public class LoginUseCase(IAuthRepository authRepository, IJwtService jwtService) : ILoginUseCase
    {
        public async Task<LoginResponseDto?> ExecuteAsync(LoginRequestDto loginRequest)
        {
            var user = await authRepository.ValidateCredentialsAsync(loginRequest.Email, loginRequest.Password);

            if (user == null)
            {
                return null;
            }

            var token = jwtService.GenerateToken(user);

            return new LoginResponseDto(token, user.Email.Endereco, user.NomeCompleto, user.Tipo.ToString());
        }
    }
}
