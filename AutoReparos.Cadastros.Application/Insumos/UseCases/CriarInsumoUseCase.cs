using AutoReparos.Cadastros.Application.Insumos.DTOs.Request;
using AutoReparos.Cadastros.Application.Insumos.DTOs.Response;
using AutoReparos.Cadastros.Application.Insumos.Mappers;
using AutoReparos.Cadastros.Application.Insumos.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Insumos.Entities;
using AutoReparos.Cadastros.Domain.Insumos.Repositories;

namespace AutoReparos.Cadastros.Application.Insumos.UseCases
{
    public class CriarInsumoUseCase(IInsumoRepository repository) : ICriarInsumoUseCase
    {
        public async Task<InsumoDto> ExecuteAsync(CriarInsumoDto dto)
        {
            var insumo = new Insumo(dto.Nome, dto.Descricao, dto.Valor, dto.QuantidadeEstoque);
            await repository.Create(insumo);
            return InsumoMapper.ToDto(insumo);
        }
    }
}
