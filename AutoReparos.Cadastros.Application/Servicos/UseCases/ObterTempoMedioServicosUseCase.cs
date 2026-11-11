using AutoReparos.Cadastros.Application.Servicos.DTOs.Response;
using AutoReparos.Cadastros.Application.Servicos.Mappers;
using AutoReparos.Cadastros.Application.Servicos.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Servicos.Repositories;

namespace AutoReparos.Cadastros.Application.Servicos.UseCases
{
    public class ObterTempoMedioServicosUseCase(IServicoRepository repository) : IObterTempoMedioServicosUseCase
    {
        public async Task<IEnumerable<TempoMedioServicoDto>> ExecuteAsync()
        {
            var result = await repository.GetTempoMedio();
            return result.Select(ServicoMapper.ToTempoMedioDto);
        }
    }
}
