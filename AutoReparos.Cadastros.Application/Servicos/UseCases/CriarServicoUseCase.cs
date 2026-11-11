using AutoReparos.Cadastros.Application.Servicos.DTOs.Request;
using AutoReparos.Cadastros.Application.Servicos.DTOs.Response;
using AutoReparos.Cadastros.Application.Servicos.Mappers;
using AutoReparos.Cadastros.Application.Servicos.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Servicos.Entities;
using AutoReparos.Cadastros.Domain.Servicos.Repositories;

namespace AutoReparos.Cadastros.Application.Servicos.UseCases
{
    public class CriarServicoUseCase(IServicoRepository repository) : ICriarServicoUseCase
    {
        public async Task<ServicoDto> ExecuteAsync(CriarServicoDto dto)
        {
            var servico = new Servico(dto.Nome, dto.Descricao, dto.ValorTabelado);
            await repository.Create(servico);
            return ServicoMapper.ToDto(servico);
        }
    }
}
