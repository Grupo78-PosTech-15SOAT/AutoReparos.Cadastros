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
    /// <summary>D-57: OS, Cobrança e Execução validam a audiência; o token do Cadastros precisa levar <c>aud</c>.</summary>
    public class JwtAudienciaTests
    {
        private static JwtService Servico(JwtSettings settings)
        {
            var opcoes = Microsoft.Extensions.Options.Options.Create(settings);
            return new JwtService(opcoes, new ChaveAssinaturaJwt(opcoes));
        }

        private static JwtSettings Settings() => new()
        {
            Issuer = "http://cadastros.test",
            ChavePrivadaPem = RSA.Create(2048).ExportPkcs8PrivateKeyPem(),
            ExpiryHours = 2
        };

        [Fact(DisplayName = "Token sem audiência configurada leva aud autoreparos")]
        public void TokenSemAudienciaConfigurada_LevaAudAutoreparos()
        {
            var token = Servico(Settings()).GenerateToken(
                new Usuario("Maria Silva", "maria@example.com", ETipoUsuario.Mecanico));

            new JwtSecurityTokenHandler().ReadJwtToken(token).Audiences
                .Should().ContainSingle().Which.Should().Be("autoreparos");
        }

        [Fact(DisplayName = "Token usa a audiência configurada em Jwt:Audience")]
        public void Token_UsaAudienciaConfigurada()
        {
            var settings = Settings();
            settings.Audience = "outra-audiencia";

            var token = Servico(settings).GenerateToken(
                new Usuario("Maria Silva", "maria@example.com", ETipoUsuario.Administrador));

            new JwtSecurityTokenHandler().ReadJwtToken(token).Audiences
                .Should().ContainSingle().Which.Should().Be("outra-audiencia");
        }
    }
}
