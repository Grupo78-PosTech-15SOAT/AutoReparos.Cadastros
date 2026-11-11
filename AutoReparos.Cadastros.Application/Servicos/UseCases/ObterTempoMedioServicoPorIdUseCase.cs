using AutoReparos.Cadastros.Application.Servicos.DTOs.Response;
using AutoReparos.Cadastros.Application.Servicos.Mappers;
using AutoReparos.Cadastros.Application.Servicos.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Servicos.Repositories;

namespace AutoReparos.Cadastros.Application.Servicos.UseCases
{
    public class ObterTempoMedioServicoPorIdUseCase(IServicoRepository repository) : IObterTempoMedioServicoPorIdUseCase
    {
        public async Task<TempoMedioServicoDto?> ExecuteAsync(Guid id)
        {
            var result = await repository.GetTempoMedioById(id);
            return result is null ? null : ServicoMapper.ToTempoMedioDto(result.Value);
        }
    }
}
