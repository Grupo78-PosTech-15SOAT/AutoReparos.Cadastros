namespace AutoReparos.Cadastros.Application.Auth.DTOs.Response
{
    public record LoginResponseDto(string Token, string Email, string NomeCompleto, string Role);
}
