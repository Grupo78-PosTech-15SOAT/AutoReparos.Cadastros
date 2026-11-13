namespace AutoReparos.Cadastros.Application.Clientes.DTOs.Response
{
    /// <summary>
    /// Cliente autenticado e o JWT RS256 emitido pelo Cadastros (D-30).
    /// </summary>
    public record ClienteAutenticadoDto(ClienteDto Cliente, string Token);
}
