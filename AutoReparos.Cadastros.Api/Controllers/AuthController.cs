using AutoReparos.Cadastros.Application.Auth.DTOs.Request;
using AutoReparos.Cadastros.Application.Auth.UseCases.Interfaces;

namespace AutoReparos.Cadastros.Api.Controllers
{
    public class AuthController(ILoginUseCase loginUseCase)
    {
        public async Task<IResult> Login(LoginRequestDto loginRequest)
        {
            var result = await loginUseCase.ExecuteAsync(loginRequest);

            if (result == null)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(result);
        }
    }
}
