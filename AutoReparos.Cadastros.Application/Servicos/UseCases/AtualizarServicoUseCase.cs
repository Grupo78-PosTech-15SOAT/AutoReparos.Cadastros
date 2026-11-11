using AutoReparos.Cadastros.Application.Servicos.DTOs.Request;
using AutoReparos.Cadastros.Application.Servicos.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Servicos.Repositories;
using AutoReparos.Cadastros.Domain.Shared.Exceptions;

namespace AutoReparos.Cadastros.Application.Servicos.UseCases
{
    public class AtualizarServicoUseCase(IServicoRepository repository) : IAtualizarServicoUseCase
    {
        public async Task ExecuteAsync(Guid id, AtualizarServicoDto dto)
        {
            var servico = await repository.GetById(id)
                ?? throw new NotFoundException("Serviço não encontrado.");

            servico.Atualizar(dto.Nome, dto.Descricao, dto.ValorTabelado);
            await repository.Update(servico);
        }
    }
}
