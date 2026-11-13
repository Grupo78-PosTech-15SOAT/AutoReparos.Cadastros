using AutoReparos.Cadastros.Domain.Clientes.Entities;
using AutoReparos.Cadastros.Domain.Usuarios.Entities;

namespace AutoReparos.Cadastros.Application.Auth.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(Usuario usuario);

        /// <summary>
        /// Emite o token do cliente (role <c>Cliente</c>) para o portal, assinado com a chave ativa do Cadastros.
        /// </summary>
        string GenerateToken(Cliente cliente);
    }
}
