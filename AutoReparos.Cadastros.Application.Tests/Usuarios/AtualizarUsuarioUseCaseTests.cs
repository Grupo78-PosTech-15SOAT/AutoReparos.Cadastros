using AutoReparos.Cadastros.Application.Usuarios.DTOs.Request;
using AutoReparos.Cadastros.Application.Usuarios.UseCases;
using AutoReparos.Cadastros.Domain.Shared.Exceptions;
using AutoReparos.Cadastros.Domain.Usuarios.Entities;
using AutoReparos.Cadastros.Domain.Usuarios.Enums;
using AutoReparos.Cadastros.Domain.Usuarios.Repositories;
using FluentAssertions;
using NSubstitute;

namespace AutoReparos.Cadastros.Application.Tests.Usuarios
{
    public class AtualizarUsuarioUseCaseTests
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly AtualizarUsuarioUseCase _useCase;

        public AtualizarUsuarioUseCaseTests()
        {
            _usuarioRepository = Substitute.For<IUsuarioRepository>();
            _useCase = new AtualizarUsuarioUseCase(_usuarioRepository);
        }

        [Fact(DisplayName = "Update Usuario Successfully")]
        public async Task Update_WhenUserExists_ShouldUpdateAndCallUpdateAsync()
        {
            var usuarioId = Guid.NewGuid();
            var usuario = new Usuario("Antigo", "test@test.com", ETipoUsuario.Atendente);
            var dto = new UsuarioUpdateDto("Novo Nome", ETipoUsuario.Administrador);

            _usuarioRepository.GetByIdAsync(usuarioId).Returns(usuario);

            await _useCase.ExecuteAsync(usuarioId, dto);

            usuario.NomeCompleto.Should().Be(dto.NomeCompleto);
            usuario.Tipo.Should().Be(dto.Tipo);
            await _usuarioRepository.Received(1).UpdateAsync(usuario);
        }

        [Fact(DisplayName = "Update Usuario Not Found Should Throw Exception")]
        public async Task Update_WhenUserDoesNotExist_ShouldThrowNotFound()
        {
            var usuarioId = Guid.NewGuid();
            var dto = new UsuarioUpdateDto("Novo Nome", ETipoUsuario.Administrador);
            _usuarioRepository.GetByIdAsync(usuarioId).Returns((Usuario?)null);

            Func<Task> action = async () => await _useCase.ExecuteAsync(usuarioId, dto);

            await action.Should().ThrowAsync<NotFoundException>();
            await _usuarioRepository.DidNotReceive().UpdateAsync(Arg.Any<Usuario>());
        }
    }
}
