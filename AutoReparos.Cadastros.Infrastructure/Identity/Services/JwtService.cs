using AutoReparos.Cadastros.Application.Auth.Services.Interfaces;
using AutoReparos.Cadastros.Domain.Clientes.Entities;
using AutoReparos.Cadastros.Domain.Usuarios.Entities;
using AutoReparos.Cadastros.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AutoReparos.Cadastros.Infrastructure.Identity.Services;

public class JwtService(IOptions<JwtSettings> jwtOptions, ChaveAssinaturaJwt chave) : IJwtService
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public string GenerateToken(Usuario usuario)
    {
        if (usuario == null) throw new ArgumentNullException(nameof(usuario));

        return Assinar(
        [
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email.Endereco),
            new Claim(ClaimTypes.Name, usuario.NomeCompleto),
            new Claim(ClaimTypes.Role, usuario.Tipo.ToString())
        ]);
    }

    public string GenerateToken(Cliente cliente)
    {
        if (cliente == null) throw new ArgumentNullException(nameof(cliente));

        return Assinar(
        [
            new Claim(ClaimTypes.NameIdentifier, cliente.Id.ToString()),
            new Claim("sub", cliente.Id.ToString()),
            new Claim(ClaimTypes.Name, cliente.Nome),
            new Claim(ClaimTypes.Email, cliente.Email.Endereco),
            new Claim("cpf", cliente.Documento.Valor),
            new Claim(ClaimTypes.Role, "Cliente")
        ]);
    }

    private string Assinar(IEnumerable<Claim> claims)
    {
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = chave.Issuer,
            Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiryHours),
            SigningCredentials = new SigningCredentials(chave.ChavePrivada, SecurityAlgorithms.RsaSha256)
        };

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(handler.CreateToken(descriptor));
    }
}
