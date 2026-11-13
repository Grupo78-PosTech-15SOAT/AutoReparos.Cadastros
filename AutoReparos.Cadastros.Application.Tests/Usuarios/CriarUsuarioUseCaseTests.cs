using AutoReparos.Cadastros.Application.Usuarios.DTOs.Request;
using AutoReparos.Cadastros.Application.Usuarios.UseCases;
using AutoReparos.Cadastros.Domain.Usuarios.Entities;
using AutoReparos.Cadastros.Domain.Usuarios.Enums;
using AutoReparos.Cadastros.Domain.Usuarios.Repositories;
using FluentAssertions;
using NSubstitute;

namespace AutoReparos.Cadastros.Application.Tests.Usuarios
{
    public class CriarUsuarioUseCaseTests
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly CriarUsuarioUseCase _useCase;

        public CriarUsuarioUseCaseTests()
        {
            _usuarioRepository = Substitute.For<IUsuarioRepository>();
            _useCase = new CriarUsuarioUseCase(_usuarioRepository);
        }

        [Fact(DisplayName = "Create Usuario Successfully")]
        public async Task Create_WithValidData_ShouldReturnDto()
        {
            var dto = new UsuarioCreateDto("Novo Usuario", "novo@test.com", "Pass123!", ETipoUsuario.Atendente);

            var result = await _useCase.ExecuteAsync(dto);

            result.Should().NotBeNull();
            result.NomeCompleto.Should().Be(dto.NomeCompleto);
            result.Email.Should().Be(dto.Email);
            await _usuarioRepository.Received(1).CreateAsync(Arg.Any<Usuario>(), dto.Password);
        }
    }
}
