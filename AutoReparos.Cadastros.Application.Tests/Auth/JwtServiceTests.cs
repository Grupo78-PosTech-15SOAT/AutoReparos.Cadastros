using AutoReparos.Cadastros.Domain.Usuarios.Entities;
using AutoReparos.Cadastros.Domain.Usuarios.Enums;
using AutoReparos.Cadastros.Infrastructure.Identity.Services;
using AutoReparos.Cadastros.Infrastructure.Settings;
using FluentAssertions;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using Xunit;

namespace AutoReparos.Cadastros.Application.Tests.Auth
{
    public class JwtServiceTests
    {
        private readonly JwtSettings _jwtSettings;
        private readonly JwtService _jwtService;

        public JwtServiceTests()
        {
            _jwtSettings = new JwtSettings
            {
                Issuer = "http://cadastros.test",
                ChavePrivadaPem = RSA.Create(2048).ExportPkcs8PrivateKeyPem(),
                ExpiryHours = 2
            };

            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();
            var opcoes = Microsoft.Extensions.Options.Options.Create(_jwtSettings);
            _jwtService = new JwtService(opcoes, new ChaveAssinaturaJwt(opcoes));
        }

        [Fact(DisplayName = "GenerateToken Should Return Valid Token Successfully")]
        public void GenerateToken_ShouldReturnValidTokenSuccessfully()
        {
            // Arrange
            var usuario = new Usuario("John Doe", "john.doe@example.com", ETipoUsuario.Administrador);

            // Act
            var token = _jwtService.GenerateToken(usuario);

            // Assert
            token.Should().NotBeNullOrWhiteSpace();

            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.CanReadToken(token).Should().BeTrue();
        }

        [Fact(DisplayName = "GenerateToken Should Contain Expected Claims")]
        public void GenerateToken_ShouldContainExpectedClaims()
        {
            // Arrange
            var usuario = new Usuario("John Doe", "john.doe@example.com", ETipoUsuario.Administrador);

            // Act
            var token = _jwtService.GenerateToken(usuario);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Find claims and print/inspect them in case of failure
            var claimTypes = jwtToken.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
            var claimsString = string.Join(", ", claimTypes);

            // Check NameIdentifier Claim
            var nameIdentifierClaim = jwtToken.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier ||
                c.Type == "nameid" ||
                c.Type == "sub" ||
                c.Type.EndsWith("nameidentifier"));
            nameIdentifierClaim.Should().NotBeNull($"because NameIdentifier claim should be present. Found claims: [{claimsString}]");
            nameIdentifierClaim!.Value.Should().Be(usuario.Id.ToString());

            // Check Email Claim
            var emailClaim = jwtToken.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Email ||
                c.Type == "email" ||
                c.Type.EndsWith("emailaddress"));
            emailClaim.Should().NotBeNull($"because Email claim should be present. Found claims: [{claimsString}]");
            emailClaim!.Value.Should().Be(usuario.Email.Endereco);

            // Check Name Claim
            var nameClaim = jwtToken.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Name ||
                c.Type == "unique_name" ||
                c.Type == "name" ||
                c.Type.EndsWith("name"));
            nameClaim.Should().NotBeNull($"because Name claim should be present. Found claims: [{claimsString}]");
            nameClaim!.Value.Should().Be(usuario.NomeCompleto);

            // Check Role Claim
            var roleClaim = jwtToken.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Role ||
                c.Type == "role" ||
                c.Type.EndsWith("role"));
            roleClaim.Should().NotBeNull($"because Role claim should be present. Found claims: [{claimsString}]");
            roleClaim!.Value.Should().Be(usuario.Tipo.ToString());
        }

        [Fact(DisplayName = "GenerateToken Should Sign With RS256 And Carry The Active Key Id")]
        public void GenerateToken_ShouldSignWithRs256AndCarryKid()
        {
            var usuario = new Usuario("John Doe", "john.doe@example.com", ETipoUsuario.Administrador);

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(_jwtService.GenerateToken(usuario));

            jwtToken.Header.Alg.Should().Be("RS256");
            jwtToken.Header.Kid.Should().NotBeNullOrWhiteSpace();
            jwtToken.Issuer.Should().Be("http://cadastros.test");
        }

        [Fact(DisplayName = "ChaveAssinaturaJwt Should Accept A PEM With Literal Backslash-n Line Breaks")]
        public void ChaveAssinaturaJwt_ShouldAcceptPemWithLiteralLineBreaks()
        {
            var emUmaLinha = RSA.Create(2048).ExportPkcs8PrivateKeyPem().Replace("\r\n", "\n").Replace("\n", "\\n");
            var opcoes = Microsoft.Extensions.Options.Options.Create(new JwtSettings { Issuer = "http://cadastros.test", ChavePrivadaPem = emUmaLinha });

            var chave = new ChaveAssinaturaJwt(opcoes);

            emUmaLinha.Should().NotContain("\n");
            chave.Kid.Should().NotBeNullOrWhiteSpace();
        }

        [Fact(DisplayName = "GenerateToken With Null Usuario Should Throw ArgumentNullException")]
        public void GenerateToken_WithNullUsuario_ShouldThrowArgumentNullException()
        {
            // Arrange & Act
            Action act = () => _jwtService.GenerateToken((Usuario)null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
