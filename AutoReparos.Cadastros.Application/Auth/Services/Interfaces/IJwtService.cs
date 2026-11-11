using AutoReparos.Cadastros.Domain.Usuarios.Entities;

namespace AutoReparos.Cadastros.Application.Auth.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(Usuario usuario);
    }
}
