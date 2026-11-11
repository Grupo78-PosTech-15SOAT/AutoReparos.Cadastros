using AutoReparos.Cadastros.Application.OrdensServicos.DTOs.Request;
using AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Core;
using AutoReparos.Cadastros.Domain.OrdensServicos.Entities;
using AutoReparos.Cadastros.Domain.OrdensServicos.Repositories;
using FluentAssertions;
using NSubstitute;

namespace AutoReparos.Cadastros.Application.Tests.OrdensServicos.Core
{
    public class ListarOrdensServicoUseCaseTests
    {
        private readonly IOrdemServicoRepository _repository;
        private readonly ListarOrdensServicoUseCase _useCase;

        public ListarOrdensServicoUseCaseTests()
        {
            _repository = Substitute.For<IOrdemServicoRepository>();
            _useCase = new ListarOrdensServicoUseCase(_repository);
        }

        [Fact(DisplayName = "GetAll Should Return Paged Result")]
        public async Task GetAll_ShouldReturnPagedResult()
        {
            var ordens = new List<OrdemServico> { new(Guid.NewGuid(), Guid.NewGuid(), "obs") };
            var request = new OrdemServicoPagedRequest { PageNumber = 1, PageSize = 10 };

            _repository.GetAll(request.ClienteId, request.VeiculoId, request.Status, request.Skip, request.PageSize)
                .Returns((ordens, ordens.Count));

            var result = await _useCase.ExecuteAsync(request);

            result.Items.Should().HaveCount(1);
            result.TotalItems.Should().Be(1);
        }
    }
}
