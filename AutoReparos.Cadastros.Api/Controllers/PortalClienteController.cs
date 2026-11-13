using AutoReparos.Cadastros.Application.Clientes.UseCases.Interfaces;
using System.Security.Claims;

namespace AutoReparos.Cadastros.Api.Controllers;

public class PortalClienteController(IObterMeusVeiculosUseCase obterMeusVeiculosUseCase)
{
    public async Task<IResult> GetMeusVeiculos(ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        var clienteId = ObterClienteId(user);
        if (clienteId == null)
            return Results.Unauthorized();

        var veiculos = await obterMeusVeiculosUseCase.ExecuteAsync(clienteId.Value, cancellationToken);
        return Results.Ok(veiculos);
    }

    private static Guid? ObterClienteId(ClaimsPrincipal user)
    {
        var idStr = user.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? user.FindFirstValue("sub");

        return Guid.TryParse(idStr, out var id) ? id : null;
    }
}
