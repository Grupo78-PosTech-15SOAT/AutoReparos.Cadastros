using AutoReparos.Cadastros.Application.Insumos.DTOs.Request;
using AutoReparos.Cadastros.Application.Insumos.UseCases;
using AutoReparos.Cadastros.Domain.Insumos.Entities;
using AutoReparos.Cadastros.Domain.Insumos.Repositories;
using FluentAssertions;
using NSubstitute;

namespace AutoReparos.Cadastros.Application.Tests.Insumos
{
    public class ListarInsumosUseCaseTests
    {
        private readonly IInsumoRepository _repository;
        private readonly ListarInsumosUseCase _useCase;

        public ListarInsumosUseCaseTests()
        {
            _repository = Substitute.For<IInsumoRepository>();
            _useCase = new ListarInsumosUseCase(_repository);
        }

        [Fact(DisplayName = "GetAll Should Return Paged Result")]
        public async Task GetAll_ShouldReturnPagedResult()
        {
            var insumos = new List<Insumo> { new("Óleo 5W30", "Óleo sintético", 45m, 10) };
            var request = new InsumoPagedRequest { PageNumber = 1, PageSize = 10 };

            _repository.GetAll(request.Nome, request.Skip, request.PageSize).Returns((insumos, insumos.Count));

            var result = await _useCase.ExecuteAsync(request);

            result.Items.Should().HaveCount(1);
            result.TotalItems.Should().Be(1);
        }
    }
}
