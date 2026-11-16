using AutoReparos.Cadastros.Api.Controllers;
using AutoReparos.Cadastros.Application.Auth.DTOs.Request;
using AutoReparos.Cadastros.Application.Auth.DTOs.Response;

namespace AutoReparos.Cadastros.Api.Endpoints
{
    public static class AuthEndpoint
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/auth")
                .WithTags("Autenticação");

            group.MapPost("/login", (LoginRequestDto loginRequest, AuthController controller) => controller.Login(loginRequest))
                .WithName("Login")
                .WithSummary("Realiza o login do usuário")
                .WithDescription("Valida as credenciais e retorna o token JWT")
                .Produces<LoginResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized);
        }
    }
}
