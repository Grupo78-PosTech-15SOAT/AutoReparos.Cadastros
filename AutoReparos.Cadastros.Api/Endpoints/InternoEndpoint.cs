using AutoReparos.Cadastros.Application.Clientes.DTOs.Request;
using AutoReparos.Cadastros.Application.Clientes.DTOs.Response;
using AutoReparos.Cadastros.Application.Clientes.UseCases.Interfaces;

namespace AutoReparos.Cadastros.Api.Endpoints
{
    public static class InternoEndpoint
    {
        /// <summary>
        /// Rotas de uso exclusivo do cluster, protegidas por NetworkPolicy e pela chave interna.
        /// </summary>
        public static void MapInternoEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/internal")
                .WithTags("Interno")
                .RequireAuthorization("ChaveInterna");

            group.MapPost("/clientes/autenticar", async (AutenticarClienteDto dto, IAutenticarClienteUseCase useCase) =>
            {
                var resultado = await useCase.ExecuteAsync(dto);
                return resultado.Status switch
                {
                    StatusAutenticacaoCliente.Autenticado => Results.Ok(resultado.Cliente),
                    StatusAutenticacaoCliente.Inativo => Results.StatusCode(StatusCodes.Status403Forbidden),
                    _ => Results.NotFound()
                };
            })
            .WithName("AutenticarClienteInterno")
            .WithSummary("Autentica o cliente por CPF e e-mail e emite o JWT")
            .Produces<ClienteAutenticadoDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
        }
    }
}
