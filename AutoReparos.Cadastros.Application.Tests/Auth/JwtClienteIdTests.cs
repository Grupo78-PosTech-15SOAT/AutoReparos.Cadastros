using AutoReparos.Cadastros.Domain.Clientes.Entities;
using AutoReparos.Cadastros.Domain.Usuarios.Entities;
using AutoReparos.Cadastros.Domain.Usuarios.Enums;
using AutoReparos.Cadastros.Infrastructure.Identity.Services;
using AutoReparos.Cadastros.Infrastructure.Settings;
using FluentAssertions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Xunit;

namespace AutoReparos.Cadastros.Application.Tests.Auth
{
    /// <summary>D-63: a Cobrança lê o claim <c>clienteId</c> para aprovar o orçamento e abrir o checkout do Cliente.</summary>
    public class JwtClienteIdTests
    {
        private static JwtService Servico()
        {
            var opcoes = Microsoft.Extensions.Options.Options.Create(new JwtSettings
            {
                Issuer = "http://cadastros.test",
                ChavePrivadaPem = RSA.Create(2048).ExportPkcs8PrivateKeyPem(),
                ExpiryHours = 2
            });
            return new JwtService(opcoes, new ChaveAssinaturaJwt(opcoes));
        }

        [Fact(DisplayName = "Token do Cliente leva clienteId igual ao id do cliente")]
        public void TokenDoCliente_LevaClienteIdIgualAoIdDoCliente()
        {
            var cliente = new Cliente("João Silva", "52998224725", "11999999999", "joao@teste.com");

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(Servico().GenerateToken(cliente));

            jwt.Claims.Where(c => c.Type == "clienteId").Should().ContainSingle()
                .Which.Value.Should().Be(cliente.Id.ToString());
            jwt.Subject.Should().Be(cliente.Id.ToString());
        }

        [Fact(DisplayName = "Token de usuário da oficina não leva clienteId")]
        public void TokenDeUsuario_NaoLevaClienteId()
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(Servico().GenerateToken(
                new Usuario("Maria Silva", "maria@example.com", ETipoUsuario.Atendente)));

            jwt.Claims.Should().NotContain(c => c.Type == "clienteId");
        }
    }
}
