using AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Core;
using AutoReparos.Cadastros.Domain.OrdensServicos.Entities;
using AutoReparos.Cadastros.Domain.OrdensServicos.Enums;
using AutoReparos.Cadastros.Domain.OrdensServicos.Repositories;
using FluentAssertions;
using NSubstitute;

namespace AutoReparos.Cadastros.Application.Tests.OrdensServicos.Core
{
    public class ObterOrdemServicoPorIdUseCaseTests
    {
        private readonly IOrdemServicoRepository _repository;
        private readonly AutoReparos.Cadastros.Domain.Usuarios.Repositories.IUsuarioRepository _usuarioRepository;
        private readonly ObterOrdemServicoPorIdUseCase _useCase;

        public ObterOrdemServicoPorIdUseCaseTests()
        {
            _repository = Substitute.For<IOrdemServicoRepository>();
            _usuarioRepository = Substitute.For<AutoReparos.Cadastros.Domain.Usuarios.Repositories.IUsuarioRepository>();
            _useCase = new ObterOrdemServicoPorIdUseCase(_repository, _usuarioRepository);
        }

        [Fact(DisplayName = "GetById When OrdemServico Exists Should Return Dto With ResponsavelNome")]
        public async Task GetById_WhenOrdemServicoExists_ShouldReturnDto()
        {
            var mecanicoId = Guid.NewGuid().ToString();
            var mecanico = new AutoReparos.Cadastros.Domain.Usuarios.Entities.Usuario("Mecânico Pedro", "mecanico@autoreparos.com", AutoReparos.Cadastros.Domain.Usuarios.Enums.ETipoUsuario.Mecanico);
            var os = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), "obs");
            os.IniciarDiagnostico(mecanicoId);

            _repository.GetById(os.Id).Returns(os);
            _usuarioRepository.GetByIdAsync(Guid.Parse(mecanicoId)).Returns(mecanico);

            var result = await _useCase.ExecuteAsync(os.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(os.Id);
            result.ResponsavelId.Should().Be(mecanicoId);
            result.ResponsavelNome.Should().Be("Mecânico Pedro");
        }

        [Fact(DisplayName = "GetById When OrdemServico Has Servicos And Insumos Should Return Dto With Items")]
        public async Task GetById_WhenOrdemServicoHasServicosEInsumos_ShouldReturnDtoComItens()
        {
            var os = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), "obs");
            var servico = new OrdemServicoServico(os.Id, Guid.NewGuid(), 150m);
            var insumo = new OrdemServicoInsumo(os.Id, Guid.NewGuid(), "Óleo 5W30", 45m, 2, EOrigemInsumo.Estoque);
            os.AdicionarServico(servico);
            os.AdicionarInsumo(insumo);

            _repository.GetById(os.Id).Returns(os);

            var result = await _useCase.ExecuteAsync(os.Id);

            result.Should().NotBeNull();
            result!.Servicos.Should().ContainSingle(s => s.Id == servico.Id && s.ServicoId == servico.ServicoId && s.ValorCobrado == servico.ValorCobrado);
            result.Insumos.Should().ContainSingle(i => i.Id == insumo.Id && i.InsumoId == insumo.InsumoId && i.ValorTotal == insumo.ValorTotal);
        }

        [Fact(DisplayName = "GetById When OrdemServico Does Not Exist Should Return Null")]
        public async Task GetById_WhenOrdemServicoDoesNotExist_ShouldReturnNull()
        {
            var id = Guid.NewGuid();
            _repository.GetById(id).Returns((OrdemServico?)null);

            var result = await _useCase.ExecuteAsync(id);

            result.Should().BeNull();
        }
    }
}
