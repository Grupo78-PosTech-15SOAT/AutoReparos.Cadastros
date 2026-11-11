using AutoReparos.Cadastros.Application.Servicos.DTOs.Response;
using AutoReparos.Cadastros.Application.Servicos.Mappers;
using AutoReparos.Cadastros.Application.Servicos.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Servicos.Repositories;

namespace AutoReparos.Cadastros.Application.Servicos.UseCases
{
    public class ObterServicoPorIdUseCase(IServicoRepository repository) : IObterServicoPorIdUseCase
    {
        public async Task<ServicoDto?> ExecuteAsync(Guid id)
        {
            var servico = await repository.GetById(id);
            return servico is null ? null : ServicoMapper.ToDto(servico);
        }
    }
}
