namespace AutoReparos.Cadastros.Application.Clientes.DTOs.Request
{
    /// <summary>
    /// Credenciais do cliente enviadas pela AuthLambda ao endpoint interno.
    /// </summary>
    public record AutenticarClienteDto(string Cpf, string Email);
}
